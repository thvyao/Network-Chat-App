using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace ChatShared.Protocol
{
	public interface IChatMessageSerializer
	{
        string Serialize(ChatMessage message);
        ChatMessage Deserialize(string payload);
        Task SendAsync(NetworkStream stream, ChatMessage message, CancellationToken cancellationToken = default);
        Task<ChatMessage> ReceiveAsync(NetworkStream stream, CancellationToken cancellationToken = default);
    }
}