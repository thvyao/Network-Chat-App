using System;
using System.Threading;
using System.Threading.Tasks;
using ChatShared.Protocol;

namespace ChatClient.Networking
{
    public interface IChatConnection : IDisposable
    {
        bool IsConnected { get; }
        event EventHandler<ChatMessage> MessageReceived;
        event EventHandler<Exception> ConnectionClosed;
        Task SendAsync(ChatMessage message, CancellationToken cancellationToken = default);
        void StartReceiving();
        void Disconnect();
    }
}
