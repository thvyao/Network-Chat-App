using System;
using System.Runtime.Serialization;

namespace ChatShared.Protocol
{
    public enum MessageType
    {
        Login,
        Logout,
        TextMessage,
        ReplyMessage,
        ForwardMessage,
        EmojiMessage,
        RequestUserList,
        PrivateMessage,
        Register,
        LoginResult,
        RegisterResult,
        UserList,
        UserJoined,
        UserLeft,
        ServerMessage,
        MessageDelivered,
        Error
    }
    [DataContract]
    public class ChatMessage
    {
        [DataMember(Name = "id")]
        public string Id { get; set; } = Guid.NewGuid().ToString();

        [DataMember(Name = "type")]
        public MessageType Type { get; set; }

        [DataMember(Name = "sender")]
        public string Sender { get; set; }

        [DataMember(Name = "recipient")]
        public string Recipient { get; set; }

        [DataMember(Name = "content")]
        public string Content { get; set; }

        [DataMember(Name = "timestamp")]
        public DateTime Timestamp { get; set; } = DateTime.Now;

        [DataMember(Name = "replyToId")]
        public string ReplyToId { get; set; }

        [DataMember(Name = "replyToContent")]
        public string ReplyToContent { get; set; }

        [DataMember(Name = "replyToSender")]
        public string ReplyToSender { get; set; }

        [DataMember(Name = "forwardFrom")]
        public string ForwardFrom { get; set; }

        [DataMember(Name = "userList")]
        public string[] UserList { get; set; }

        [DataMember(Name = "success")]
        public bool Success { get; set; }

        [DataMember(Name = "avatarIndex")]
        public int AvatarIndex { get; set; }

        [DataMember(Name = "password")]
        public string Password { get; set; }
    }
}
