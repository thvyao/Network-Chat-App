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

            lblServerIPValue.Text = serverIP;
            lblPortValue.Text = serverPort.ToString();

            SetConnectedState(true);
            picOwnAvatar.Image = GetAvatar(username, 44);
            lblOwnUsername.Text = username;
            lblOwnStatus.Text = "Trực tuyến";
            lblOwnStatusDot.ForeColor = Color.FromArgb(52, 168, 83);

            AddSystemMessage($"Đã đăng nhập thành công vào máy chủ {serverIP}:{serverPort} với tài khoản '{username}'.");

            if (initialLoginMsg != null && initialLoginMsg.UserList != null)
            {
                UpdateUserList(initialLoginMsg.UserList);
            }

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
            AddSystemMessage(error == null ? "Máy chủ đã đóng kết nối." : "Kết nối máy chủ bị gián đoạn.");
            Disconnect();
        }
        #region Connection Logic
        public void RequestLogout()
        {
            var result = MessageBox.Show(
                "Bạn có chắc chắn muốn đăng xuất khỏi tài khoản?",
                "Xác nhận đăng xuất",
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

            try
            {
                cts?.Cancel();
            }
            catch { }

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

            lblOwnStatus.Text = "Ngoại tuyến";
            lblOwnStatusDot.ForeColor = Color.Gray;

            InitUserList();
            SwitchToBroadcast();

            AddSystemMessage("Đã ngắt kết nối khỏi máy chủ.");
        }
        private void SetConnectedState(bool connected)
        {
            if (connected)
            {
                btnSend.Enabled = true;
                txtMessage.Enabled = true;
                btnEmoji.Enabled = true;
            }
            else
            {
                btnSend.Enabled = false;
                txtMessage.Enabled = false;
                btnEmoji.Enabled = false;

                CancelReply();
            }
            lblConnectionStatusValue.Text = connected ? "Đã kết nối" : "Chưa kết nối";
        }

        #endregion

        #region Message Dispatching
        private void ProcessReceivedMessage(ChatMessage msg)
        {
            switch (msg.Type)
            {
                case MessageType.LoginResult:
                    if (!msg.Success)
                    {
                        MessageBox.Show($"Đăng nhập không thành công: {msg.Content}", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        Disconnect();
                    }
                    break;

                case MessageType.UserList:
                    if (msg.UserList != null)
                    {
                        UpdateUserList(msg.UserList);
                    }
                    break;

                case MessageType.UserJoined:
                    if (!string.Equals(msg.Sender, username, StringComparison.OrdinalIgnoreCase))
                    {
                        AddSystemMessage($"{msg.Sender} đã tham gia cuộc trò chuyện.");
                        AddUserIfNotExists(msg.Sender);
                    }
                    break;

                case MessageType.UserLeft:
                    if (!string.Equals(msg.Sender, username, StringComparison.OrdinalIgnoreCase))
                    {
                        AddSystemMessage($"{msg.Sender} đã rời cuộc trò chuyện.");
                        RemoveUser(msg.Sender);
                    }
                    break;

                case MessageType.ServerMessage:
                    AddSystemMessage(msg.Content);
                    break;

                case MessageType.TextMessage:
                case MessageType.EmojiMessage:
                case MessageType.ReplyMessage:
                case MessageType.ForwardMessage:
                case MessageType.PrivateMessage:
                    var displayMsg = new DisplayMessage
                    {
                        Id = msg.Id,
                        Sender = msg.Sender,
                        Content = msg.Content,
                        Timestamp = msg.Timestamp,
                        IsOwn = string.Equals(msg.Sender, username, StringComparison.OrdinalIgnoreCase),
                        ReplyToSender = msg.ReplyToSender,
                        ReplyToContent = msg.ReplyToContent,
                        ForwardFrom = msg.ForwardFrom,
                        Recipient = msg.Recipient,
                        IsSystem = false
                    };
                    AddMessageToChat(displayMsg);
                    break;

                case MessageType.Error:
                    AddSystemMessage($"[Lỗi từ máy chủ] {msg.Content}");
                    break;
            }
        }

        #endregion

        #region Messaging: Send, Reply, Forward, Display
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
                MessageBox.Show("Bạn chưa kết nối tới máy chủ.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                ChatMessage msg = new ChatMessage
                {
                    Sender = username,
                    Recipient = currentRecipient,
                    Content = text,
                    Timestamp = DateTime.Now
                };

                if (replyingTo != null)
                {
                    msg.Type = MessageType.ReplyMessage;
                    msg.ReplyToId = replyingTo.Id;
                    msg.ReplyToSender = replyingTo.Sender;
                    msg.ReplyToContent = replyingTo.Content;
                }
                else if (currentRecipient != null)
                {
                    msg.Type = MessageType.PrivateMessage;
                }
                else
                {
                    msg.Type = MessageType.TextMessage;
                }

                if (chatConnection == null)
                    throw new InvalidOperationException("Kết nối tới máy chủ chưa sẵn sàng.");

                await chatConnection.SendAsync(msg, cts.Token);

                var displayMsg = new DisplayMessage
                {
                    Id = msg.Id,
                    Sender = username,
                    Recipient = currentRecipient,
                    Content = text,
                    Timestamp = msg.Timestamp,
                    IsOwn = true,
                    ReplyToSender = msg.ReplyToSender,
                    ReplyToContent = msg.ReplyToContent,
                    ForwardFrom = msg.ForwardFrom,
                    IsSystem = false
                };

                AddMessageToChat(displayMsg);

                txtMessage.Clear();
                CancelReply();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi gửi tin nhắn: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}