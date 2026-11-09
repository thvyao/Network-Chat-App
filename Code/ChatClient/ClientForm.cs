using System;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using ChatShared.Protocol;
using ChatClient.Networking;

namespace ChatClient
{
    public partial class ClientForm : Form
    {
        private TcpClient client;
        private NetworkStream stream;
        private CancellationTokenSource cts;
        private string username;
        private IChatConnection chatConnection;

        private string currentRecipient = null;
        public bool WantsRelogin { get; set; } = false;

        public ClientForm()
        {
            InitializeComponent();
        }

        public ClientForm(string authUser, TcpClient authClient, NetworkStream authStream, ChatMessage initialLoginMsg, string serverIP, int serverPort)
            : this()
        {
            AttachAuthenticatedSession(authUser, authClient, authStream, initialLoginMsg, serverIP, serverPort);
        }

        public void AttachAuthenticatedSession(string authUser, TcpClient authClient, NetworkStream authStream, ChatMessage initialLoginMsg, string serverIP, int serverPort)
        {
            client = authClient;
            stream = authStream;
            username = authUser;
            cts = new CancellationTokenSource();

            SetConnectedState(true);
            this.Text = $"Chat Client - {username} (Đã kết nối {serverIP}:{serverPort})";

            if (IsHandleCreated)
            {
                StartSessionNetwork();
            }
            else
            {
                this.Shown += (s, e) => StartSessionNetwork();
            }
        }

        private void StartSessionNetwork()
        {
            if (client == null || !client.Connected) return;
            StartConnectionEvents();

            var reqList = new ChatMessage
            {
                Type = MessageType.RequestUserList,
                Sender = username,
                Timestamp = DateTime.Now
            };
            _ = chatConnection?.SendAsync(reqList, cts?.Token ?? CancellationToken.None);
        }

        private void StartConnectionEvents()
        {
            chatConnection?.Dispose();
            chatConnection = new ChatClientConnection(client, stream);
            chatConnection.MessageReceived += OnConnectionMessageReceived;
            chatConnection.ConnectionClosed += OnConnectionClosed;
            chatConnection.StartReceiving();
        }

        private void OnConnectionMessageReceived(object sender, ChatMessage message)
        {
            if (IsDisposed) return;
            if (!IsHandleCreated)
            {
                this.HandleCreated += (s, e) =>
                {
                    if (!IsDisposed)
                        BeginInvoke((Action)(() => ProcessReceivedMessage(message)));
                };
                return;
            }
            BeginInvoke((Action)(() => ProcessReceivedMessage(message)));
        }

        private void OnConnectionClosed(object sender, Exception error)
        {
            if (IsDisposed) return;
            if (!IsHandleCreated)
            {
                this.HandleCreated += (s, e) =>
                {
                    if (!IsDisposed)
                        BeginInvoke((Action)(() => HandleConnectionClosed(error)));
                };
                return;
            }
            BeginInvoke((Action)(() => HandleConnectionClosed(error)));
        }

        private void HandleConnectionClosed(Exception error)
        {
            if (cts == null || cts.IsCancellationRequested) return;
            Disconnect();
            MessageBox.Show("Mất kết nối tới máy chủ.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        public void RequestLogout()
        {
            var result = MessageBox.Show(
                "Bạn có chắc chắn muốn đăng xuất?",
                "Xác nhận",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                WantsRelogin = true;
                Disconnect();
                this.Close();
            }
        }

        private void Disconnect()
        {
            if (client == null && stream == null) return;

            try
            {
                if (client != null && client.Connected && stream != null && cts != null && !cts.IsCancellationRequested)
                {
                    var logoutMsg = new ChatMessage
                    {
                        Type = MessageType.Logout,
                        Sender = username,
                        Timestamp = DateTime.Now
                    };
                    _ = chatConnection?.SendAsync(logoutMsg);
                }
            }
            catch { }

            try { cts?.Cancel(); } catch { }

            CleanupNetwork();
            OnDisconnected();
        }

        private void CleanupNetwork()
        {
            try { stream?.Close(); stream?.Dispose(); } catch { }
            try { client?.Close(); } catch { }
            try { cts?.Dispose(); } catch { }
            stream = null;
            client = null;
            cts = null;
        }

        private void OnDisconnected()
        {
            SetConnectedState(false);
            this.Text = "Chat Client - Ngoại tuyến";
        }

        private void SetConnectedState(bool connected)
        {
            btnSend.Enabled = connected;
            txtMessage.Enabled = connected;
        }

        private void ProcessReceivedMessage(ChatMessage msg)
        {
            switch (msg.Type)
            {
                case MessageType.LoginResult:
                    if (!msg.Success)
                    {
                        MessageBox.Show($"Đăng nhập thất bại: {msg.Content}", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        Disconnect();
                    }
                    break;

                case MessageType.UserList:
                    if (msg.UserList != null)
                    {
                        lstMessages.Items.Add($"[Hệ thống] Danh sách online: {string.Join(", ", msg.UserList)}");
                    }
                    break;

                case MessageType.UserJoined:
                    lstMessages.Items.Add($"[Hệ thống] {msg.Sender} đã tham gia.");
                    break;

                case MessageType.UserLeft:
                    lstMessages.Items.Add($"[Hệ thống] {msg.Sender} đã rời phòng.");
                    break;

                case MessageType.TextMessage:
                case MessageType.PrivateMessage:
                    lstMessages.Items.Add($"[{msg.Timestamp:HH:mm:ss}] {msg.Sender}: {msg.Content}");
                    break;

                case MessageType.Error:
                    MessageBox.Show($"[Lỗi máy chủ] {msg.Content}", "Lỗi mạng", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    break;
            }
        }

        private async void btnSend_Click(object sender, EventArgs e)
        {
            await SendTextMessage();
        }

        private async Task SendTextMessage()
        {
            string text = txtMessage.Text.Trim();
            if (string.IsNullOrEmpty(text)) return;

            if (client == null || !client.Connected || stream == null)
            {
                MessageBox.Show("Chưa kết nối tới máy chủ.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                ChatMessage msg = new ChatMessage
                {
                    Sender = username,
                    Recipient = currentRecipient,
                    Content = text,
                    Timestamp = DateTime.Now,
                    Type = currentRecipient != null ? MessageType.PrivateMessage : MessageType.TextMessage
                };

                if (chatConnection == null)
                    throw new InvalidOperationException("Kết nối tới máy chủ chưa sẵn sàng.");

                await chatConnection.SendAsync(msg, cts.Token);
                lstMessages.Items.Add($"[{DateTime.Now:HH:mm:ss}] Tôi: {text}");
                txtMessage.Clear();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi gửi tin nhắn: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ClientForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            Disconnect();
        }
    }
}