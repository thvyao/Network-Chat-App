using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace ChatShared.Protocol
{
    public sealed class JsonChatMessageSerializer : IChatMessageSerializer
    {
        public string Serialize(ChatMessage message) => MessageSerializer.Serialize(message);

        public ChatMessage Deserialize(string payload) => MessageSerializer.Deserialize(payload);

        public Task SendAsync(NetworkStream stream, ChatMessage message, CancellationToken cancellationToken = default)
        {
            return MessageSerializer.SendMessageAsync(stream, message, cancellationToken);
        }

        public Task<ChatMessage> ReceiveAsync(NetworkStream stream, CancellationToken cancellationToken = default)
        {
            return MessageSerializer.ReceiveMessageAsync(stream, cancellationToken);
        }
    }

}

