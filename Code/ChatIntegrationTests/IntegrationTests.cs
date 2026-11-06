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
    }
}