
using ChatServer;
using ChatShared.Protocol;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Concurrent;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using ChatServer.Infrastructure;
using ChatShared.Helpers;
using ChatClient;
using System.Runtime.ExceptionServices;

namespace ChatIntegrationTests
{
    [TestClass]
    public class ServerCoreUnitTests
    {
        // UNIT_CORE_01: Kiểm tra Server xử lý Logout.
        [TestMethod]
        public async Task UNIT_CORE_01_ProcessMessage_Logout()
        {
            ServerForm server = new ServerForm();
            CancellationTokenSource clientCts =
                new CancellationTokenSource();
            TcpClient tcpClient = new TcpClient();

            try
            {
                ServerForm.ClientInfo client =
                    new ServerForm.ClientInfo
                    {
                        Username = "userA",
                        TcpClient = tcpClient,
                        Stream = null,
                        ConnectedAt = DateTime.Now,
                        ClientCts = clientCts
                    };

                ChatMessage message = new ChatMessage
                {
                    Type = MessageType.Logout,
                    Sender = "userA",
                    Timestamp = DateTime.Now
                };

                await server.ProcessMessage(client, message);

                Assert.IsTrue(
                    clientCts.IsCancellationRequested,
                    "FAIL: Server chưa hủy phiên khi Logout."
                );
            }
            finally
            {
                tcpClient.Dispose();
                clientCts.Dispose();
                ((IDisposable)server).Dispose();
            }
        }

        // CORE_02: Kiểm tra gửi tin công khai cho B và C.
        // Đây là Integration Test ở mức Server/TCP.
        [TestMethod]
        [Timeout(15000)]
        public async Task CORE_02_ProcessMessage_GuiTinCongKhai()
        {
            await Task.Run(async () =>
            {
                ServerForm server = null;
                TcpListener listener = null;

                TcpClient clientB = null;
                TcpClient clientC = null;
                TcpClient serverB = null;
                TcpClient serverC = null;
                TcpClient senderSocket = null;

                CancellationTokenSource ctsB = null;
                CancellationTokenSource ctsC = null;
                CancellationTokenSource senderCts = null;

                try
                {
                    server = new ServerForm();
                    SynchronizationContext
                        .SetSynchronizationContext(null);

                    // Tạo TCP Listener cục bộ
                    listener = new TcpListener(
                        IPAddress.Loopback, 0);

                    listener.Start();

                    int port =
                        ((IPEndPoint)listener.LocalEndpoint).Port;

                    // Tạo kết nối TCP cho Client B
                    clientB = new TcpClient();

                    Task<TcpClient> acceptB =
                        listener.AcceptTcpClientAsync();

                    await clientB.ConnectAsync(
                        IPAddress.Loopback, port)
                        .WaitAsync(TimeSpan.FromSeconds(5))
                        .ConfigureAwait(false);

                    serverB = await acceptB
                        .WaitAsync(TimeSpan.FromSeconds(5))
                        .ConfigureAwait(false);

                    // Tạo kết nối TCP cho Client C
                    clientC = new TcpClient();

                    Task<TcpClient> acceptC =
                        listener.AcceptTcpClientAsync();

                    await clientC.ConnectAsync(
                        IPAddress.Loopback, port)
                        .WaitAsync(TimeSpan.FromSeconds(5))
                        .ConfigureAwait(false);

                    serverC = await acceptC
                        .WaitAsync(TimeSpan.FromSeconds(5))
                        .ConfigureAwait(false);

                    ctsB = new CancellationTokenSource();
                    ctsC = new CancellationTokenSource();

                    // Thông tin Client B
                    ServerForm.ClientInfo infoB =
                        new ServerForm.ClientInfo
                        {
                            Username = "userB",
                            TcpClient = serverB,
                            Stream = serverB.GetStream(),
                            ClientCts = ctsB,
                            ConnectedAt = DateTime.Now
                        };

                    // Thông tin Client C
                    ServerForm.ClientInfo infoC =
                        new ServerForm.ClientInfo
                        {
                            Username = "userC",
                            TcpClient = serverC,
                            Stream = serverC.GetStream(),
                            ClientCts = ctsC,
                            ConnectedAt = DateTime.Now
                        };

                    // Lấy danh sách Client của Server
                    FieldInfo field =
                        typeof(ServerForm).GetField(
                            "connectedClients",
                            BindingFlags.Instance |
                            BindingFlags.NonPublic
                        );

                    Assert.IsNotNull(
                        field,
                        "Không tìm thấy connectedClients."
                    );

                    var clients =
                        field.GetValue(server) as
                        ConcurrentDictionary<
                            string,
                            ServerForm.ClientInfo>;

                    Assert.IsNotNull(
                        clients,
                        "Danh sách Client không đúng kiểu."
                    );

                    // Thêm B và C vào danh sách online
                    Assert.IsTrue(
                        clients.TryAdd("userB", infoB)
                    );

                    Assert.IsTrue(
                        clients.TryAdd("userC", infoC)
                    );

                    // Giả lập Client A gửi tin nhắn
                    senderSocket = new TcpClient();
                    senderCts = new CancellationTokenSource();

                    ServerForm.ClientInfo sender =
                        new ServerForm.ClientInfo
                        {
                            Username = "userA",
                            TcpClient = senderSocket,
                            ClientCts = senderCts
                        };

                    ChatMessage message = new ChatMessage
                    {
                        Id = "CORE02",
                        Type = MessageType.TextMessage,
                        Sender = "userA",
                        Recipient = null,
                        Content = "Xin chao B va C",
                        Timestamp = DateTime.Now
                    };

                    // Gọi hàm chính của Server
                    // Giới hạn thời gian tối đa 5 giây
                    await server.ProcessMessage(
                        sender, message)
                        .WaitAsync(TimeSpan.FromSeconds(5))
                        .ConfigureAwait(false);

                    // Nhận tin tại Client B và C
                    using (var timeout =
                        new CancellationTokenSource(
                            TimeSpan.FromSeconds(5)))
                    {
                        ChatMessage receivedB =
                            await MessageSerializer
                                .ReceiveMessageAsync(
                                    clientB.GetStream(),
                                    timeout.Token)
                                .WaitAsync(
                                    TimeSpan.FromSeconds(5))
                                .ConfigureAwait(false);

                        ChatMessage receivedC =
                            await MessageSerializer
                                .ReceiveMessageAsync(
                                    clientC.GetStream(),
                                    timeout.Token)
                                .WaitAsync(
                                    TimeSpan.FromSeconds(5))
                                .ConfigureAwait(false);

                        // Kiểm tra B và C có nhận được tin
                        Assert.IsNotNull(
                            receivedB,
                            "Client B không nhận được tin."
                        );

                        Assert.IsNotNull(
                            receivedC,
                            "Client C không nhận được tin."
                        );

                        // Kiểm tra loại tin nhắn
                        Assert.AreEqual(
                            MessageType.TextMessage,
                            receivedB.Type
                        );

                        Assert.AreEqual(
                            MessageType.TextMessage,
                            receivedC.Type
                        );

                        // Kiểm tra nội dung nhận được
                        Assert.AreEqual(
                            message.Content,
                            receivedB.Content
                        );

                        Assert.AreEqual(
                            message.Content,
                            receivedC.Content
                        );

                        // Kiểm tra người gửi
                        Assert.AreEqual(
                            "userA",
                            receivedB.Sender
                        );

                        Assert.AreEqual(
                            "userA",
                            receivedC.Sender
                        );
                    }
                }
                finally
                {
                    // Đóng tất cả kết nối sau test
                    listener?.Stop();
                    clientB?.Dispose();
                    clientC?.Dispose();
                    serverB?.Dispose();
                    serverC?.Dispose();
                    senderSocket?.Dispose();

                    ctsB?.Dispose();
                    ctsC?.Dispose();
                    senderCts?.Dispose();

                    if (server != null)
                    {
                        ((IDisposable)server).Dispose();
                    }
                }
            })
            .WaitAsync(TimeSpan.FromSeconds(15))
            .ConfigureAwait(false);
        }

        // Repository giả lập: không đọc/ghi file thật.
        private sealed class MemoryAccountRepository : IAccountRepository
        {
            public string StoragePath => "memory";

            public bool ThrowOnSave { get; set; }

            public int SaveCount { get; private set; }

            public List<UserAccount> StoredAccounts { get; private set; }
                = new List<UserAccount>();

            public IReadOnlyList<UserAccount> Load()
            {
                return new List<UserAccount>(StoredAccounts);
            }

            public void Save(IEnumerable<UserAccount> accounts)
            {
                SaveCount++;

                if (ThrowOnSave)
                    throw new InvalidOperationException(
                        "Gia lap loi luu tai khoan");

                StoredAccounts = new List<UserAccount>(accounts);
            }
        }


        // UNIT_CORE_03: Đăng ký thành công phải lưu tài khoản.
        [TestMethod]
        public void UNIT_CORE_03_Register_DangKyThanhCong()
        {
            var repository = new MemoryAccountRepository();
            var manager = new AccountManager(repository);

            bool result = manager.Register(
                "userA", "123456", 1, out string error);

            Assert.IsTrue(result, error);
            Assert.AreEqual(1, manager.TotalAccounts);
            Assert.AreEqual(1, repository.SaveCount);
            Assert.AreEqual(1, repository.StoredAccounts.Count);
            Assert.AreEqual(
                "userA",
                repository.StoredAccounts[0].Username);
        }


        // UNIT_CORE_04: Lưu thất bại phải hủy đăng ký.
        [TestMethod]
        public void UNIT_CORE_04_Register_LuuThatBai()
        {
            var repository = new MemoryAccountRepository
            {
                ThrowOnSave = true
            };

            var manager = new AccountManager(repository);

            bool result = manager.Register(
                "userB", "123456", 1, out string error);

            Assert.IsFalse(result);
            Assert.AreEqual(0, manager.TotalAccounts);
            Assert.IsFalse(manager.IsRegistered("userB"));

            StringAssert.Contains(
                error,
                "Không thể lưu tài khoản");
        }


        // UNIT_CORE_05: Đăng nhập sai mật khẩu.
        [TestMethod]
        public void UNIT_CORE_05_VerifyLogin_SaiMatKhau()
        {
            var repository = new MemoryAccountRepository();
            var manager = new AccountManager(repository);

            Assert.IsTrue(
                manager.Register(
                    "userC", "123456", 1, out string registerError),
                registerError);

            int saveCountBeforeLogin = repository.SaveCount;

            bool result = manager.VerifyLogin(
                "userC",
                "sai-mat-khau",
                out UserAccount account,
                out string error);

            Assert.IsFalse(result);

            StringAssert.Contains(
                error,
                "Mật khẩu không chính xác");

            Assert.AreEqual(
                saveCountBeforeLogin,
                repository.SaveCount);
        }


        // UNIT_CORE_06: Đăng nhập đúng phải cập nhật LastLogin.
        [TestMethod]
        public void UNIT_CORE_06_VerifyLogin_DangNhapThanhCong()
        {
            var repository = new MemoryAccountRepository();
            var manager = new AccountManager(repository);

            Assert.IsTrue(
                manager.Register(
                    "userD", "123456", 2, out string registerError),
                registerError);

            bool result = manager.VerifyLogin(
                "userD",
                "123456",
                out UserAccount account,
                out string error);

            Assert.IsTrue(result, error);
            Assert.IsNotNull(account);
            Assert.AreEqual("userD", account.Username);
            Assert.IsTrue(account.LastLogin.HasValue);

            // Một lần lưu khi đăng ký,
            // một lần lưu khi đăng nhập thành công.
            Assert.AreEqual(2, repository.SaveCount);
        }


        // UNIT_CORE_07: Không cho đăng ký trùng tên,
        // kể cả khi khác chữ hoa/chữ thường.
        [TestMethod]
        public void UNIT_CORE_07_Register_KhongChoTrungTen()
        {
            var repository = new MemoryAccountRepository();
            var manager = new AccountManager(repository);

            Assert.IsTrue(
                manager.Register(
                    "userE", "123456", 1, out string firstError),
                firstError);

            bool result = manager.Register(
                "USERE", "654321", 1, out string error);

            Assert.IsFalse(result);
            Assert.AreEqual(1, manager.TotalAccounts);
            Assert.AreEqual(1, repository.SaveCount);

            StringAssert.Contains(
                error,
                "đã được đăng ký");
        }

      
        private static object CreateLimiter()
        {
            var type = typeof(AccountManager).Assembly.GetType(
                "ChatServer.LoginAttemptLimiter",
                throwOnError: true);

            return Activator.CreateInstance(
                type,
                nonPublic: true);
        }

        private static bool LimiterAllows(
            object limiter,
            string username,
            string ip,
            out string error)
        {
            object[] args = { username, ip, null };

            bool result = (bool)limiter.GetType()
                .GetMethod("TryAllow")
                .Invoke(limiter, args);

            error = (string)args[2];
            return result;
        }

        private static void LimiterRecordFailure(
            object limiter,
            string username,
            string ip)
        {
            limiter.GetType()
                .GetMethod("RecordFailure")
                .Invoke(limiter, new object[] { username, ip });
        }

        private static void LimiterClear(
            object limiter,
            string username,
            string ip)
        {
            limiter.GetType()
                .GetMethod("Clear")
                .Invoke(limiter, new object[] { username, ip });
        }


        // UNIT_CORE_08
        // Sai 4 lần vẫn được thử, lần thứ 5 bị khóa.
        [TestMethod]
        public void UNIT_CORE_08_DangNhapSai5Lan_BiKhoa()
        {
            var limiter = CreateLimiter();
            string username = "userA";
            string ip = "192.0.2.10";

            for (int i = 0; i < 4; i++)
            {
                LimiterRecordFailure(limiter, username, ip);
            }

            Assert.IsTrue(
                LimiterAllows(
                    limiter, username, ip, out string errorBefore),
                "Chưa đủ 5 lần sai nhưng đã bị khóa.");

            LimiterRecordFailure(limiter, username, ip);

            bool allowed = LimiterAllows(
                limiter, username, ip, out string errorAfter);

            Assert.IsFalse(allowed);

            StringAssert.Contains(
                errorAfter,
                "quá nhiều lần đăng nhập");
        }


        // UNIT_CORE_09
        // Khóa đúng tài khoản và địa chỉ IP.
        [TestMethod]
        public void UNIT_CORE_09_KhoaTaiKhoan_KhongAnhHuongNguoiKhac()
        {
            var limiter = CreateLimiter();

            for (int i = 0; i < 5; i++)
            {
                LimiterRecordFailure(
                    limiter, "userA", "192.0.2.10");
            }

            // Cùng tài khoản và IP: bị khóa.
            // Tên tài khoản không phân biệt hoa/thường.
            Assert.IsFalse(
                LimiterAllows(
                    limiter,
                    "USERA",
                    "192.0.2.10",
                    out _));

            // Cùng tài khoản nhưng IP khác: được phép.
            Assert.IsTrue(
                LimiterAllows(
                    limiter,
                    "userA",
                    "192.0.2.11",
                    out _));

            // Tài khoản khác, cùng IP: được phép.
            Assert.IsTrue(
                LimiterAllows(
                    limiter,
                    "userB",
                    "192.0.2.10",
                    out _));
        }


        // UNIT_CORE_10
        // Xóa trạng thái khóa cho phép đăng nhập trở lại.
        [TestMethod]
        public void UNIT_CORE_10_Clear_XoaTrangThaiKhoa()
        {
            var limiter = CreateLimiter();
            string username = "userC";
            string ip = "192.0.2.20";

            for (int i = 0; i < 5; i++)
            {
                LimiterRecordFailure(limiter, username, ip);
            }

            Assert.IsFalse(
                LimiterAllows(
                    limiter, username, ip, out _));

            // Gọi hàm Clear thực tế của nhóm.
            LimiterClear(limiter, username, ip);

            Assert.IsTrue(
                LimiterAllows(
                    limiter, username, ip, out string error));

            Assert.IsNull(error);
        }

        // UNIT_CORE_11
        // Kiểm tra xác thực mật khẩu đúng và sai.
        [TestMethod]
        public void UNIT_CORE_11_XacThucMatKhau()
        {
            string password = "MatKhau123";
            string salt = SecurityHelper.GenerateSalt();

            string hash = SecurityHelper.HashPassword(
                password, salt);

            Assert.IsTrue(
                SecurityHelper.IsCurrentPasswordHash(hash));

            Assert.IsTrue(
                SecurityHelper.VerifyPassword(
                    password, salt, hash));

            Assert.IsFalse(
                SecurityHelper.VerifyPassword(
                    "SaiMatKhau", salt, hash));
        }


        // UNIT_CORE_12
        // Cùng mật khẩu nhưng salt khác nhau.
        [TestMethod]
        public void UNIT_CORE_12_SaltKhac_HashKhac()
        {
            string password = "MatKhau123";

            string salt1 = Convert.ToBase64String(
                new byte[16]);

            string salt2 = Convert.ToBase64String(
                new byte[]
                {
                    1, 2, 3, 4, 5, 6, 7, 8,
                    9, 10, 11, 12, 13, 14, 15, 16
                });

            string hash1 = SecurityHelper.HashPassword(
                password, salt1);

            string hash2 = SecurityHelper.HashPassword(
                password, salt2);

            Assert.AreNotEqual(hash1, hash2);

            Assert.IsTrue(
                SecurityHelper.VerifyPassword(
                    password, salt1, hash1));

            Assert.IsFalse(
                SecurityHelper.VerifyPassword(
                    password, salt2, hash1));
        }


        // UNIT_CORE_13
        // Kiểm tra từ chối hash không hợp lệ.
        [TestMethod]
        public void UNIT_CORE_13_TuChoiHashKhongHopLe()
        {
            string salt = SecurityHelper.GenerateSalt();

            string[] invalidHashes =
            {
                "",
                "PBKDF2$abc$abcd",
                "PBKDF2$100$abcd",
                "PBKDF2$210000$%%%"
            };

            foreach (string hash in invalidHashes)
            {
                Assert.IsFalse(
                    SecurityHelper.VerifyPassword(
                        "MatKhau123", salt, hash),
                    $"Hash không hợp lệ vẫn được chấp nhận: {hash}");
            }
        }

        // UNIT_CORE_14
        // Đăng nhập đúng nhưng lưu dữ liệu thất bại.
        // LastLogin phải được khôi phục.
        [TestMethod]
        public void UNIT_CORE_14_VerifyLogin_LuuThatBai()
        {
            var repository = new MemoryAccountRepository();
            var manager = new AccountManager(repository);

            Assert.IsTrue(
                manager.Register(
                    "user14", "123456", 1,
                    out string registerError),
                registerError);

            repository.ThrowOnSave = true;

            bool result = manager.VerifyLogin(
                "user14", "123456",
                out UserAccount account,
                out string error);

            Assert.IsTrue(result, error);
            Assert.IsNotNull(account);

            // Không lưu được thì hoàn tác LastLogin.
            Assert.IsNull(account.LastLogin);
            Assert.AreEqual(2, repository.SaveCount);
        }


        // UNIT_CORE_15
        // Kiểm tra nâng cấp hash mật khẩu tài khoản cũ.
        [TestMethod]
        public void UNIT_CORE_15_NangCapMatKhauCu()
        {
            string password = "123456";
            string salt = SecurityHelper.GenerateSalt();

            // Tạo hash theo cơ chế cũ của nhóm.
            byte[] legacyBytes =
                System.Security.Cryptography.SHA256.HashData(
                    System.Text.Encoding.UTF8.GetBytes(
                        salt + ":" + password));

            string legacyHash =
                Convert.ToHexString(legacyBytes)
                    .ToLowerInvariant();

            var repository = new MemoryAccountRepository();

            repository.StoredAccounts.Add(new UserAccount
            {
                Username = "oldUser",
                PasswordHash = legacyHash,
                Salt = salt,
                AvatarIndex = 1
            });

            var manager = new AccountManager(repository);

            bool result = manager.VerifyLogin(
                "oldUser", password,
                out UserAccount account,
                out string error);

            Assert.IsTrue(result, error);
            Assert.IsNotNull(account);

            // Sau khi đăng nhập, hash phải chuyển sang PBKDF2.
            Assert.IsTrue(
                SecurityHelper.IsCurrentPasswordHash(
                    account.PasswordHash));

            Assert.AreNotEqual(
                legacyHash, account.PasswordHash);

            Assert.AreEqual(1, repository.SaveCount);

            Assert.IsTrue(
                SecurityHelper.VerifyPassword(
                    password,
                    account.Salt,
                    account.PasswordHash));
        }


        // UNIT_CORE_16
        // Đăng nhập không phân biệt chữ hoa/thường
        // và tự loại khoảng trắng đầu/cuối tên.
        [TestMethod]
        public void UNIT_CORE_16_DangNhap_ChuanHoaTen()
        {
            var repository = new MemoryAccountRepository();
            var manager = new AccountManager(repository);

            Assert.IsTrue(
                manager.Register(
                    "user16", "123456", 1,
                    out string registerError),
                registerError);

            bool result = manager.VerifyLogin(
                "  USER16  ", "123456",
                out UserAccount account,
                out string error);

            Assert.IsTrue(result, error);
            Assert.IsNotNull(account);

            Assert.AreEqual(
                "user16", account.Username);

            Assert.IsTrue(
                manager.IsRegistered("  USER16  "));
        }

        // Chạy kiểm thử WinForms trên luồng STA.
        private static void RunChatUiOnSta(Action action)
        {
            Exception failure = null;

            var thread = new Thread(() =>
            {
                try
                {
                    action();
                }
                catch (Exception ex)
                {
                    failure = ex;
                }
            });

            thread.IsBackground = true;
            thread.SetApartmentState(ApartmentState.STA);
            thread.Start();

            if (!thread.Join(10000))
                Assert.Fail("Kiểm thử giao diện quá thời gian.");

            if (failure != null)
                ExceptionDispatchInfo.Capture(failure).Throw();
        }


        // UNIT_CHAT_01
        // Bong bóng bên người nhận giữ đúng nội dung tin nhắn.
        [TestMethod]
        public void UNIT_CHAT_01_HienThiNoiDungNguoiNhan()
        {
            RunChatUiOnSta(() =>
            {
                string content =
                    "Xin chào B! 😊\nKiểm tra tiếng Việt.";

                var message = new DisplayMessage
                {
                    Sender = "userA",
                    Content = content,
                    IsOwn = false,
                    Timestamp = DateTime.Now
                };

                using var bubble =
                    new MessageBubbleControl(message, null, 420);

                Assert.IsNotNull(bubble.Message);
                Assert.AreEqual(content, bubble.Message.Content);
                Assert.AreEqual("userA", bubble.Message.Sender);
                Assert.IsFalse(bubble.IsOwnMessage);
                Assert.IsTrue(bubble.Height > 0);
            });
        }


        // UNIT_CHAT_02
        // Thu nhỏ khung chat không được làm mất nội dung.
        [TestMethod]
        public void UNIT_CHAT_02_ThuNhoKhungChat_KhongMatTin()
        {
            RunChatUiOnSta(() =>
            {
                string content = string.Concat(
                    System.Linq.Enumerable.Repeat(
                        "Đây là tin nhắn dài kiểm tra hiển thị. ",
                        40));

                var message = new DisplayMessage
                {
                    Sender = "userA",
                    Content = content,
                    IsOwn = false,
                    Timestamp = DateTime.Now
                };

                using var bubble =
                    new MessageBubbleControl(message, null, 620);

                int heightBefore = bubble.Height;

                // Giả lập thu nhỏ vùng hiển thị
                bubble.UpdateWidth(200);

                Assert.AreEqual(200, bubble.Width);
                Assert.IsTrue(
                    bubble.Height > heightBefore,
                    "Khung thu nhỏ nhưng không tăng chiều cao.");

                Assert.AreEqual(
                    content,
                    bubble.Message.Content);
            });
        }

    }
}