using System.IO;
using ChatServer;
using ChatShared.Protocol;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ChatIntegrationTests
{
    [TestClass]
    public class IntegrationTests
    {
        // INT-PRO-01:
        // Kiểm tra chuyển tin nhắn sang JSON và đọc lại.
        [TestMethod]
        public void INT_PRO_01_KiemTraChuyenJson()
        {
            // tạo tin nhắn thử
            ChatMessage tinNhanGui = new ChatMessage();

            tinNhanGui.Id = "01";
            tinNhanGui.Type = MessageType.TextMessage;
            tinNhanGui.Sender = "test01";
            tinNhanGui.Recipient = "test02";
            tinNhanGui.Content = "Xin chào";

            // Chuyển tin nhắn thành JSON
            string json =
                MessageSerializer.Serialize(tinNhanGui);

            // đọc tin nhắn từ JSON
            ChatMessage tinNhanNhan =
                MessageSerializer.Deserialize(json);

            // so  sánh dữ liệu trước và sau
            Assert.IsNotNull(tinNhanNhan);
            Assert.AreEqual(tinNhanGui.Id, tinNhanNhan.Id);
            Assert.AreEqual(tinNhanGui.Type, tinNhanNhan.Type);
            Assert.AreEqual(tinNhanGui.Sender, tinNhanNhan.Sender);
            Assert.AreEqual(tinNhanGui.Recipient, tinNhanNhan.Recipient);
            Assert.AreEqual(tinNhanGui.Content, tinNhanNhan.Content);
        }

        // INT-PRO-02:
        // kiểm tra tiếng Việt và emoji.
        [TestMethod]
        public void INT_PRO_02_KiemTraTiengVietVaEmoji()
        {
            // Tạo tin nhắn thử
            ChatMessage tinNhanGui = new ChatMessage();

            tinNhanGui.Content = "Xin chào Việt Nam 😀";

            // chuyển sang JSON và đọc lại
            string json =
                MessageSerializer.Serialize(tinNhanGui);

            ChatMessage tinNhanNhan =
                MessageSerializer.Deserialize(json);

            // kiểm tra nội dung có giữ nguyên không
            Assert.IsNotNull(tinNhanNhan);

            Assert.AreEqual(
                tinNhanGui.Content,
                tinNhanNhan.Content
            );
        }

        // INT-ACC-01:
        // kiểm tra đăng ký tài khoản và lưu file.
        [TestMethod]
        public void INT_ACC_01_KiemTraDangKyTaiKhoan()
        {
            // Tạo thư mục dùng riêng cho test
            string thuMucTest =
                Path.Combine(
                    Path.GetTempPath(),
                    "ChatAppTestData"
                );

            // Xóa dữ liệu test cũ nếu có
            if (Directory.Exists(thuMucTest))
            {
                Directory.Delete(thuMucTest, true);
            }

            Directory.CreateDirectory(thuMucTest);

            // tạo bộ quản lý tài khoản
            AccountManager quanLyTaiKhoan =
                new AccountManager(thuMucTest);

            string thongBaoLoi;

            // thử đăng ký tài khoản
            bool ketQua = quanLyTaiKhoan.Register(
                "test01",
                "1234",
                0,
                out thongBaoLoi
            );

            // kiểm tra đăng ký thành công
            Assert.IsTrue(ketQua);

            // Kiểm tra tài khoản đã tồn tại
            Assert.IsTrue(
                quanLyTaiKhoan.IsRegistered("test01")
            );

            // Kiểm tra file accounts.txt đã được tạo
            string fileTaiKhoan =
                Path.Combine(
                    thuMucTest,
                    "accounts.txt"
                );

            Assert.IsTrue(
                File.Exists(fileTaiKhoan)
            );

            // Xóa dữ liệu test sau khi kiểm tra
            Directory.Delete(thuMucTest, true);
        }
        // INT-ACC-02:
        // Kiểm tra tài khoản vẫn được đọc lại sau khi tạo AccountManager mới.
        [TestMethod]
        public void INT_ACC_02_KiemTraDocLaiTaiKhoan()
        {
            // tạo thư mục riêng cho test
            string thuMucTest = Path.Combine(
                Path.GetTempPath(),
                "ChatAppTest_ACC02"
            );

            // xóa dữ liệu cũ nếu có
            if (Directory.Exists(thuMucTest))
            {
                Directory.Delete(thuMucTest, true);
            }

            Directory.CreateDirectory(thuMucTest);

            // tạo AccountManager thứ nhất và đăng ký tài khoản
            AccountManager quanLyThuNhat =
                new AccountManager(thuMucTest);

            string thongBaoLoi;

            bool ketQuaDangKy = quanLyThuNhat.Register(
                "test01",
                "1234",
                0,
                out thongBaoLoi
            );

            Assert.IsTrue(ketQuaDangKy);

            // tạo AccountManager thứ hai từ cùng thư mục
            AccountManager quanLyThuHai =
                new AccountManager(thuMucTest);

            // kiểm tra tài khoản đã được đọc lại
            Assert.IsTrue(
                quanLyThuHai.IsRegistered("test01")
            );

            // xóa dữ liệu sau khi test
            Directory.Delete(thuMucTest, true);
        }
        // INT-ACC-03:
        // Kiểm tra không cho đăng ký username trùng.
        [TestMethod]
        public void INT_ACC_03_KiemTraTenDangNhapTrung()
        {
            // tạo đường dẫn thư mục test
            string thuMucTest = Path.Combine(
                Path.GetTempPath(),
                "ChatAppTest_ACC03"
            );

            // Xóa thư mục cũ
            if (Directory.Exists(thuMucTest))
            {
                Directory.Delete(thuMucTest, true);
            }

            // tạo thư mục mới
            Directory.CreateDirectory(thuMucTest);

            // tạo bộ quản lý tài khoản
            AccountManager quanLyTaiKhoan =
                new AccountManager(thuMucTest);

            string thongBaoLanMot;

            // đăng ký tài khoản lần thứ nhất
            bool ketQuaLanMot = quanLyTaiKhoan.Register(
                "test01",
                "1234",
                0,
                out thongBaoLanMot
            );

            Assert.IsTrue(ketQuaLanMot);

            string thongBaoLanHai;

            // đăng  ký lại cùng username
            bool ketQuaLanHai = quanLyTaiKhoan.Register(
                "test01",
                "5678",
                0,
                out thongBaoLanHai
            );

            // lần đăng ký thứ hai phải thất bại
            Assert.IsFalse(ketQuaLanHai);

            // kiểm tra thông báo lỗi
            Assert.AreEqual(
                "Tên đăng nhập này đã được đăng ký. Vui lòng chọn tên khác.",
                thongBaoLanHai
            );

            // xóa dữ liệu test
            Directory.Delete(thuMucTest, true);
        }
        // INT-ACC-04:
        // Kiểm tra username rỗng, khoảng trắng và quá ngắn.
        [TestMethod]
        public void INT_ACC_04_KiemTraTenDangNhapKhongHopLe()
        {
            // tạo đường dẫn thư mục test
            string thuMucTest = Path.Combine(
                Path.GetTempPath(),
                "ChatAppTest_ACC04"
            );

            // xóa thư mục cũ nếu có
            if (Directory.Exists(thuMucTest))
            {
                Directory.Delete(thuMucTest, true);
            }

            // tạo thư mục mới
            Directory.CreateDirectory(thuMucTest);

            // tạo bộ quản lý tài khoản
            AccountManager quanLyTaiKhoan =
                new AccountManager(thuMucTest);

            // trường hợp 1: username rỗng
            string thongBaoRong;

            bool ketQuaRong = quanLyTaiKhoan.Register(
                "",
                "1234",
                0,
                out thongBaoRong
            );

            Assert.IsFalse(ketQuaRong);

            Assert.AreEqual(
                "Tên đăng nhập không được để trống.",
                thongBaoRong
            );

            // trường hợp 2: username chỉ có khoảng trắng
            string thongBaoKhoangTrang;

            bool ketQuaKhoangTrang = quanLyTaiKhoan.Register(
                "   ",
                "1234",
                0,
                out thongBaoKhoangTrang
            );

            Assert.IsFalse(ketQuaKhoangTrang);

            Assert.AreEqual(
                "Tên đăng nhập không được để trống.",
                thongBaoKhoangTrang
            );

            // trường hợp 3: username chỉ có 1 ký tự
            string thongBaoQuaNgan;

            bool ketQuaQuaNgan = quanLyTaiKhoan.Register(
                "a",
                "1234",
                0,
                out thongBaoQuaNgan
            );

            Assert.IsFalse(ketQuaQuaNgan);

            Assert.AreEqual(
                "Tên đăng nhập phải có ít nhất 2 ký tự.",
                thongBaoQuaNgan
            );

            // xóa dữ liệu test
            Directory.Delete(thuMucTest, true);
        }
        // INT-ACC-05:
        // Kiểm tra các tên đăng nhập được hệ thống bảo lưu.
        [TestMethod]
        public void INT_ACC_05_KiemTraTenBaoLuu()
        {
            // tạo đường dẫn thư mục test
            string thuMucTest = Path.Combine(
                Path.GetTempPath(),
                "ChatAppTest_ACC05"
            );

            // xóa thư mục cũ nếu có
            if (Directory.Exists(thuMucTest))
            {
                Directory.Delete(thuMucTest, true);
            }

            // tạo thư mục mới
            Directory.CreateDirectory(thuMucTest);

            // tạo bộ quản lý tài khoản
            AccountManager quanLyTaiKhoan =
                new AccountManager(thuMucTest);

            // kiểm tra tên admin
            string thongBaoAdmin;

            bool ketQuaAdmin = quanLyTaiKhoan.Register(
                "admin",
                "1234",
                0,
                out thongBaoAdmin
            );

            Assert.IsFalse(ketQuaAdmin);

            Assert.AreEqual(
                "Tên đăng nhập này là từ khóa bảo lưu của hệ thống.",
                thongBaoAdmin
            );

            // kiểm tra tên server
            string thongBaoServer;

            bool ketQuaServer = quanLyTaiKhoan.Register(
                "server",
                "1234",
                0,
                out thongBaoServer
            );

            Assert.IsFalse(ketQuaServer);

            Assert.AreEqual(
                "Tên đăng nhập này là từ khóa bảo lưu của hệ thống.",
                thongBaoServer
            );

            // kiểm tra tên hệ thống
            string thongBaoHeThong;

            bool ketQuaHeThong = quanLyTaiKhoan.Register(
                "hệ thống",
                "1234",
                0,
                out thongBaoHeThong
            );

            Assert.IsFalse(ketQuaHeThong);

            Assert.AreEqual(
                "Tên đăng nhập này là từ khóa bảo lưu của hệ thống.",
                thongBaoHeThong
            );
            // kiểm tra tên all
            string thongBaoAll;

            bool ketQuaAll = quanLyTaiKhoan.Register(
                "all",
                "1234",
                0,
                out thongBaoAll
            );

            Assert.IsFalse(ketQuaAll);
            Assert.AreEqual(
                "Tên đăng nhập này là từ khóa bảo lưu của hệ thống.",
                thongBaoAll
            );

            // xóa dữ liệu test
            Directory.Delete(thuMucTest, true);
        }
        // INT-ACC-06:
        // Kiểm tra username có ký tự không hợp lệ.
        [TestMethod]
        public void INT_ACC_06_KiemTraKyTuKhongHopLe()
        {
            // tạo đường dẫn thư mục test
            string thuMucTest = Path.Combine(
                Path.GetTempPath(),
                "ChatAppTest_ACC06"
            );
            // xóa thư mục cũ nếu có
            if (Directory.Exists(thuMucTest))
            {
                Directory.Delete(thuMucTest, true);
            }
            // tạo thư mục mới
            Directory.CreateDirectory(thuMucTest);

            // tạo bộ quản lý tài khoản
            AccountManager quanLyTaiKhoan =
                new AccountManager(thuMucTest);

            // trường hợp username chứa ký tự @
            string thongBaoKyTuA;

            bool ketQuaKyTuA = quanLyTaiKhoan.Register(
                "user@01",
                "1234",
                0,
                out thongBaoKyTuA
            );
            Assert.IsFalse(ketQuaKyTuA);

            Assert.AreEqual(
                "Tên đăng nhập chỉ được chứa chữ cái, chữ số, khoảng trắng, gạch dưới (_) hoặc gạch ngang (-).",
                thongBaoKyTuA
            );

            // trường hợp username chứa ký tự #
            string thongBaoKyTuB;

            bool ketQuaKyTuB = quanLyTaiKhoan.Register(
                "user#01",
                "1234",
                0,
                out thongBaoKyTuB
            );

            Assert.IsFalse(ketQuaKyTuB);

            Assert.AreEqual(
                "Tên đăng nhập chỉ được chứa chữ cái, chữ số, khoảng trắng, gạch dưới (_) hoặc gạch ngang (-).",
                thongBaoKyTuB
            );

            // xóa dữ liệu test
            Directory.Delete(thuMucTest, true);
        }
        // INT-ACC-07:
        // Kiểm tra không cho đăng ký khi mật khẩu quá ngắn.
        [TestMethod]
        public void INT_ACC_07_KiemTraMatKhauQuaNgan()
        {
            // tạo đường dẫn thư mục test
            string thuMucTest = Path.Combine(
                Path.GetTempPath(),
                "ChatAppTest_ACC07"
            );
            // xóa thư mục cũ nếu có
            if (Directory.Exists(thuMucTest))
            {
                Directory.Delete(thuMucTest, true);
            }
            // tạo thư mục mới
            Directory.CreateDirectory(thuMucTest);
            // tạo bộ quản lý tài khoản
            AccountManager quanLyTaiKhoan =
                new AccountManager(thuMucTest);
            string thongBaoLoi;
            // thử đăng ký với mật khẩu chỉ có 3 ký tự
            bool ketQua = quanLyTaiKhoan.Register(
                "test02",
                "123",
                0,
                out thongBaoLoi
            );
            // đăng ký phải thất bại
            Assert.IsFalse(ketQua);
            // kiểm tra thông báo lỗi
            Assert.AreEqual(
                "Mật khẩu phải có độ dài ít nhất 4 ký tự.",
                thongBaoLoi
            );

            // tài khoản test02 không được tạo
            bool daTonTai =
                quanLyTaiKhoan.IsRegistered("test02");

            Assert.IsFalse(daTonTai);
            // xóa dữ liệu test
            Directory.Delete(thuMucTest, true);
        }
        // INT-ACC-08:
        // Kiểm tra đăng nhập đúng sau khi đọc lại dữ liệu tài khoản.
        [TestMethod]
        public void INT_ACC_08_KiemTraDangNhapDung()
        {
            string testFolder = Path.Combine(
                Path.GetTempPath(),
                "ChatApp_INT_ACC_08"
            );

            // xóa dữ liệu cũ nếu có
            if (Directory.Exists(testFolder))
            {
                Directory.Delete(testFolder, true);
            }
            Directory.CreateDirectory(testFolder);

            // tạo và đăng ký tài khoản
            AccountManager accountManager1 =
                new AccountManager(testFolder);

            string registerError;

            bool registerResult = accountManager1.Register(
                "test01",
                "1234",
                0,
                out registerError
            );

            Assert.IsTrue(registerResult, registerError);

            // tạo AccountManager mới để đọc lại tài khoản
            AccountManager accountManager2 =
                new AccountManager(testFolder);
            UserAccount account;
            string loginError;

            bool loginResult = accountManager2.VerifyLogin(
                "test01",
                "1234",
                out account,
                out loginError
            );

            // Kiểm tra kết quả đăng nhập
            Assert.IsTrue(loginResult, loginError);
            Assert.IsNotNull(account);
            Assert.IsTrue(account.LastLogin.HasValue);
            Directory.Delete(testFolder, true);
        }

        // INT-ACC-09:
        // Kiểm tra hệ thống từ chối mật khẩu sai
        [TestMethod]
        public void INT_ACC_09_KiemTraMatKhauSai()
        {
            string testFolder = Path.Combine(
                Path.GetTempPath(),
                "ChatApp_INT_ACC_09"
            );

            if (Directory.Exists(testFolder))
            {
                Directory.Delete(testFolder, true);
            }

            Directory.CreateDirectory(testFolder);

            AccountManager accountManager =
                new AccountManager(testFolder);

            string registerError;

            bool registerResult = accountManager.Register(
                "test01",
                "1234",
                0,
                out registerError
            );

            Assert.IsTrue(registerResult, registerError);

            // thử đăng nhập bằng mật khẩu sai
            UserAccount account;
            string loginError;

            bool loginResult = accountManager.VerifyLogin(
                "test01",
                "9999",
                out account,
                out loginError
            );

            Assert.IsFalse(loginResult);

            Assert.AreEqual(
                "Mật khẩu không chính xác. Vui lòng thử lại.",
                loginError
            );
            Directory.Delete(testFolder, true);
        }
        // INT-ACC-10:
        // Kiểm tra file không lưu mật khẩu 1234 dạng thông thường.
        [TestMethod]
        public void INT_ACC_10_KiemTraKhongLuuMatKhauThuong()
        {
            string testFolder = Path.Combine(
                Path.GetTempPath(),
                "ChatApp_INT_ACC_10"
            );

            if (Directory.Exists(testFolder))
            {
                Directory.Delete(testFolder, true);
            }
            Directory.CreateDirectory(testFolder);

            AccountManager accountManager =
                new AccountManager(testFolder);

            string registerError;

            bool registerResult = accountManager.Register(
                "test01",
                "1234",
                0,
                out registerError
            );

            Assert.IsTrue(registerResult, registerError);

            string accountFile = Path.Combine(
                testFolder,
                "accounts.txt"
            );

            Assert.IsTrue(File.Exists(accountFile));

            // dòng đầu là tiêu đề, dòng thứ hai là tài khoản
            string[] lines = File.ReadAllLines(accountFile);

            Assert.IsTrue(lines.Length >= 2);

            // chia dữ liệu theo dấu |
            string[] fields = lines[1].Split('|');

            Assert.AreEqual(6, fields.Length);

            // fields[1] là mật khẩu đã băm
            // fields[2] là salt
            Assert.AreNotEqual("1234", fields[1]);
            Assert.IsFalse(string.IsNullOrEmpty(fields[1]));
            Assert.IsFalse(string.IsNullOrEmpty(fields[2]));
            Directory.Delete(testFolder, true);
        }
    }
}