using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using ChatShared.Helpers;
using ChatServer.Infrastructure;

namespace ChatServer
{
    /// <summary>
    /// Đối tượng lưu trữ thông tin một tài khoản người dùng đã đăng ký trong hệ thống.
    /// Mật khẩu được bảo vệ an toàn bằng mã băm SHA-256 kèm Salt ngẫu nhiên.
    /// </summary>
    [DataContract]
    public class UserAccount
    {
        [DataMember(Name = "username")]
        public string Username { get; set; }

        [DataMember(Name = "passwordHash")]
        public string PasswordHash { get; set; }

        [DataMember(Name = "salt")]
        public string Salt { get; set; }

        [DataMember(Name = "createdAt")]
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        [DataMember(Name = "lastLogin")]
        public DateTime? LastLogin { get; set; }

        [DataMember(Name = "avatarIndex")]
        public int AvatarIndex { get; set; }
    }

    /// <summary>
    /// Bộ quản lý tài khoản người dùng phía Máy chủ (Server).
    /// Chịu trách nhiệm xác thực đăng nhập, đăng ký tài khoản mới, kiểm tra mật khẩu băm,
    /// và lưu trữ bền vững danh sách tài khoản vào tệp văn bản 'accounts.txt'.
    /// </summary>
    public class AccountManager
    {
        private readonly IAccountRepository repository;
        private readonly object repositoryLock = new object();
        private readonly ConcurrentDictionary<string, UserAccount> accounts =
            new ConcurrentDictionary<string, UserAccount>(StringComparer.OrdinalIgnoreCase);

        public AccountManager() : this((string)null) { }

        public AccountManager(string storageDirectory)
            : this(new TextAccountRepository(storageDirectory ?? AppDomain.CurrentDomain.BaseDirectory))
        {
        }

        public AccountManager(IAccountRepository accountRepository)
        {
            repository = accountRepository ?? throw new ArgumentNullException(nameof(accountRepository));
            LoadAccounts();
        }

        /// <summary>Tổng số lượng tài khoản đã đăng ký trong hệ thống</summary>
        public int TotalAccounts => accounts.Count;

        /// <summary>
        /// Nạp danh sách tài khoản từ tệp accounts.txt.
        /// Khi chưa có tệp, hệ thống khởi tạo danh sách rỗng; người dùng tự đăng ký qua GUI.
        /// </summary>
        private void LoadAccounts()
        {
            lock (repositoryLock)
            {
                try
                {
                    var list = repository.Load();
                    accounts.Clear();
                    foreach (var acc in list)
                    {
                        if (!string.IsNullOrWhiteSpace(acc?.Username))
                        {
                            accounts[acc.Username.Trim()] = acc;
                        }
                    }
                }
                catch
                {
                    // Không tạo tài khoản có mật khẩu nhúng trong mã nguồn khi dữ liệu bị lỗi.
                }
            }
        }

        /// <summary>
        /// Lưu danh sách tài khoản hiện tại vào tệp văn bản an toàn đa luồng.
        /// </summary>
        public void SaveAccounts()
        {
            lock (repositoryLock)
            {
                SaveAccountsInternal();
            }
        }

        private void SaveAccountsInternal()
        {
            try
            {
                repository.Save(accounts.Values);
            }
            catch
            {
                // Silently handle save failure
            }
        }

        /// <summary>
        /// Đăng ký tài khoản người dùng mới vào hệ thống.
        /// </summary>
        public bool Register(string username, string password, int avatarIndex, out string errorMessage)
        {
            errorMessage = null;

            if (string.IsNullOrWhiteSpace(username))
            {
                errorMessage = "Tên đăng nhập không được để trống.";
                return false;
            }

            username = username.Trim();

            if (username.Length < 2)
            {
                errorMessage = "Tên đăng nhập phải có ít nhất 2 ký tự.";
                return false;
            }

            if (username.Length > 25)
            {
                errorMessage = "Tên đăng nhập không được dài quá 25 ký tự.";
                return false;
            }

            string[] reserved = { "server", "máy chủ", "may chu", "admin", "quản trị viên", "quan tri vien", "hệ thống", "he thong", "all", "tất cả", "tat ca" };
            if (reserved.Any(r => r.Equals(username, StringComparison.OrdinalIgnoreCase)))
            {
                errorMessage = "Tên đăng nhập này là từ khóa bảo lưu của hệ thống.";
                return false;
            }

            foreach (char c in username)
            {
                if (!char.IsLetterOrDigit(c) && c != ' ' && c != '_' && c != '-')
                {
                    errorMessage = "Tên đăng nhập chỉ được chứa chữ cái, chữ số, khoảng trắng, gạch dưới (_) hoặc gạch ngang (-).";
                    return false;
                }
            }

            if (accounts.ContainsKey(username))
            {
                errorMessage = "Tên đăng nhập này đã được đăng ký. Vui lòng chọn tên khác.";
                return false;
            }

            if (string.IsNullOrEmpty(password) || password.Length < 4)
            {
                errorMessage = "Mật khẩu phải có độ dài ít nhất 4 ký tự.";
                return false;
            }

            if (password.Length > 100)
            {
                errorMessage = "Mật khẩu không được vượt quá 100 ký tự.";
                return false;
            }

            string salt = SecurityHelper.GenerateSalt();
            string hash = SecurityHelper.HashPassword(password, salt);

            var newAccount = new UserAccount
            {
                Username = username,
                PasswordHash = hash,
                Salt = salt,
                CreatedAt = DateTime.Now,
                AvatarIndex = avatarIndex
            };

            if (accounts.TryAdd(username, newAccount))
            {
                SaveAccounts();
                return true;
            }
            else
            {
                errorMessage = "Tên đăng nhập này đã được đăng ký.";
                return false;
            }
        }

        /// <summary>
        /// Xác thực thông tin đăng nhập của người dùng.
        /// </summary>
        public bool VerifyLogin(string username, string password, out UserAccount account, out string errorMessage)
        {
            account = null;
            errorMessage = null;

            if (string.IsNullOrWhiteSpace(username))
            {
                errorMessage = "Vui lòng nhập tên đăng nhập.";
                return false;
            }

            username = username.Trim();

            if (string.IsNullOrEmpty(password))
            {
                errorMessage = "Vui lòng nhập mật khẩu.";
                return false;
            }

            if (!accounts.TryGetValue(username, out account))
            {
                errorMessage = "Tài khoản không tồn tại. Vui lòng kiểm tra lại hoặc đăng ký.";
                return false;
            }

            if (!SecurityHelper.VerifyPassword(password, account.Salt, account.PasswordHash))
            {
                errorMessage = "Mật khẩu không chính xác. Vui lòng thử lại.";
                return false;
            }

            // Update last login timestamp
            account.LastLogin = DateTime.Now;
            SaveAccounts();
            return true;
        }

        /// <summary>
        /// Kiểm tra xem tên đăng nhập đã được đăng ký trong hệ thống hay chưa.
        /// </summary>
        public bool IsRegistered(string username)
        {
            if (string.IsNullOrWhiteSpace(username)) return false;
            return accounts.ContainsKey(username.Trim());
        }

        /// <summary>
        /// Lấy danh sách toàn bộ các tài khoản đã đăng ký
        /// </summary>
        public IReadOnlyList<UserAccount> GetAllAccounts()
        {
            return accounts.Values.ToList();
        }
    }
}
