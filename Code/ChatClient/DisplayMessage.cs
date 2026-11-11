using System;

namespace ChatClient
{

    // Lớp mô hình biểu diễn dữ liệu tin nhắn phục vụ hiển thị trực tiếp lên giao diện người dùng (GUI) Client.
    // Chứa các thuộc tính tính toán hỗ trợ phân màu bong bóng, hiển thị trích dẫn (Reply) và chuyển tiếp (Forward).
    public class DisplayMessage
    {
        public string Id { get; set; } = Guid.NewGuid().ToString(); 
        public string Sender { get; set; }
        public string Content { get; set; } 
        public DateTime Timestamp { get; set; } = DateTime.Now; 
        public bool IsOwn { get; set; } 
        public string ReplyToSender { get; set; } 
        public string ReplyToContent { get; set; }
        public string ForwardFrom { get; set; } 
        public string Recipient { get; set; } 
        public bool IsSystem { get; set; } 
    }
}
