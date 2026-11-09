using System;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using ChatShared.Protocol;

namespace ChatClient.Networking
{
    /// <summary>Network-only client session. UI code consumes events instead of reading sockets directly.</summary>
    public sealed class ChatClientConnection : IChatConnection
    {
        private readonly TcpClient client;
        private readonly NetworkStream stream;
        private readonly IChatMessageSerializer serializer;
        private readonly CancellationTokenSource cancellation = new CancellationTokenSource();
        private bool started;

        public ChatClientConnection(TcpClient tcpClient, NetworkStream networkStream,
            IChatMessageSerializer messageSerializer = null)
        {
            client = tcpClient ?? throw new ArgumentNullException(nameof(tcpClient));
            stream = networkStream ?? throw new ArgumentNullException(nameof(networkStream));
            serializer = messageSerializer ?? new JsonChatMessageSerializer();
        }

        public bool IsConnected => client.Connected && !cancellation.IsCancellationRequested;

        public event EventHandler<ChatMessage> MessageReceived;
        public event EventHandler<Exception> ConnectionClosed;

        public Task SendAsync(ChatMessage message, CancellationToken cancellationToken = default)
        {
            if (message == null) throw new ArgumentNullException(nameof(message));
            return serializer.SendAsync(stream, message, cancellationToken);
        }

        public void StartReceiving()
        {
            if (started) return;
            started = true;
            _ = ReceiveLoopAsync();
        }

        private async Task ReceiveLoopAsync()
        {
            Exception failure = null;
            try
            {
                while (IsConnected)
                {
                    var message = await serializer.ReceiveAsync(stream, cancellation.Token);
                    if (message == null) break;
                    MessageReceived?.Invoke(this, message);
                }
            }
            catch (OperationCanceledException) when (cancellation.IsCancellationRequested)
            {
                return;
            }
            catch (Exception ex)
            {
                failure = ex;
            }
            finally
            {
                if (!cancellation.IsCancellationRequested)
                    ConnectionClosed?.Invoke(this, failure);
            }
        }

        public void Disconnect()
        {
            if (!cancellation.IsCancellationRequested) cancellation.Cancel();
            try { stream.Close(); } catch { }
            try { client.Close(); } catch { }
        }

        public void Dispose()
        {
            Disconnect();
            cancellation.Dispose();
        }
    }
}
