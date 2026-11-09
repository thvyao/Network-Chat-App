using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using ChatShared.Helpers;
using ChatServer.Infrastructure;

namespace ChatServer
{
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

        public int TotalAccounts => accounts.Count;


        public string StoragePath => repository.StoragePath;


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
                }
            }
        }
        public bool SaveAccounts(out string errorMessage)
        {
            lock (repositoryLock)
            {
                return SaveAccountsInternal(out errorMessage);
            }
        }

        private bool SaveAccountsInternal(out string errorMessage)
        {
            try
            {
                repository.Save(accounts.Values);
                errorMessage = null;
                return true;
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
                return false;
            }
        }
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
                if (SaveAccounts(out string saveError))
                    return true;

                accounts.TryRemove(username, out _);
                errorMessage = "Không thể lưu tài khoản. Vui lòng thử lại. Chi tiết: " + saveError;
                return false;
            }
            else
            {
                errorMessage = "Tên đăng nhập này đã được đăng ký.";
                return false;
            }
        }
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

            DateTime? previousLastLogin = account.LastLogin;
            account.LastLogin = DateTime.Now;
            if (!SaveAccounts(out _))
                account.LastLogin = previousLastLogin;
            return true;
        }
        public bool IsRegistered(string username)
        {
            if (string.IsNullOrWhiteSpace(username)) return false;
            return accounts.ContainsKey(username.Trim());
        }
        public IReadOnlyList<UserAccount> GetAllAccounts()
        {
            return accounts.Values.ToList();
        }
    }
}
