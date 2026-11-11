using System.Runtime.InteropServices;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Net.Sockets;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using ChatShared.Helpers;
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
        private readonly List<DisplayMessage> chatMessages = new List<DisplayMessage>(); 
        private ChatMessage replyingTo = null; 
        private readonly Dictionary<string, Image> avatarCache = new Dictionary<string, Image>(StringComparer.OrdinalIgnoreCase);
        private readonly Dictionary<string, UserListItem> onlineUsers =
            new Dictionary<string, UserListItem>(
                StringComparer.OrdinalIgnoreCase
            );

        private Image allChannelAvatar; 

        // Lớp biểu diễn một mục người dùng trong danh sách lstUsers.
        public class UserListItem
        {
            public string Username { get; set; }
            public bool IsAll { get; set; } 
            public int UnreadCount { get; set; } 
            public override string ToString()
            {
                return IsAll ? "Tất cả" : Username;
            }
        }

        private bool isFilteringUsers = false;
        public bool WantsRelogin { get; set; } = false;
        public ClientForm()
        {
            InitializeComponent();

            SendMessage(
                txtSearch.Handle,
                EM_SETCUEBANNER,
                IntPtr.Zero,
                "Tìm kiếm người dùng..."
            );
            pnlSettingsView.Visible = false;

            // Kích hoạt DoubleBuffering cho panel chat và listbox danh sách người dùng để chống nhấp nháy
            EnableDoubleBuffering(pnlChatFlow);
            EnableDoubleBuffering(lstUsers);
            InitializeApp();

            lstUsers.BringToFront();
            splitContainerMain.Panel1.PerformLayout();
        }

        // Khởi tạo ClientForm với phiên làm việc đã xác thực từ LoginForm.
        public ClientForm(string authUser, TcpClient authClient, NetworkStream authStream, ChatMessage initialLoginMsg, string serverIP, int serverPort)
            : this()
        {
            AttachAuthenticatedSession(authUser, authClient, authStream, initialLoginMsg, serverIP, serverPort);
        }

        // Gắn phiên làm việc đã xác thực và bắt đầu giao tiếp qua mạng TCP.
        public void AttachAuthenticatedSession(string authUser, TcpClient authClient, NetworkStream authStream, ChatMessage initialLoginMsg, string serverIP, int serverPort)
        {
            client = authClient;
            stream = authStream;
            username = authUser;
            cts = new CancellationTokenSource();

            lblServerIPValue.Text = serverIP;
            lblPortValue.Text = serverPort.ToString();

            // Cập nhật giao diện trạng thái kết nối
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

        private void InitializeApp()
        {
            allChannelAvatar = CreateGroupAvatar(36);
            picChatHeaderAvatar.Image = allChannelAvatar;

            InitUserList();

            SetConnectedState(false);
            lblOwnStatus.Text = "Ngoại tuyến";
            lblOwnStatusDot.ForeColor = Color.Gray;
            picOwnAvatar.Image = AvatarHelper.GenerateAvatar("?", 44);

            btnSend.FlatAppearance.BorderSize = 0;
            btnCancelReply.FlatAppearance.BorderSize = 0;



            // Tạo nút Đăng xuất trong Settings
            Button btnLogoutSettings = new Button
            {
                Text = "Đăng xuất",
                Width = 175,
                Dock = DockStyle.Right,
                TextAlign = ContentAlignment.MiddleCenter,
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.FromArgb(255, 237, 240),
                ForeColor = Color.FromArgb(190, 30, 55),
                Font = new Font("Segoe UI", 11F, FontStyle.Bold),
                Cursor = Cursors.Hand
            };

            btnLogoutSettings.FlatAppearance.BorderColor =
                Color.FromArgb(235, 140, 155);

            btnLogoutSettings.Click += (s, e) => RequestLogout();

            pnlSettingsFooter.Controls.Add(btnLogoutSettings);
        }


        private Image CreateGroupAvatar(int size)
        {
            Bitmap bmp = new Bitmap(size, size);
            using (Graphics g = Graphics.FromImage(bmp))
            {
                g.SmoothingMode = SmoothingMode.AntiAlias;
                using (SolidBrush b = new SolidBrush(Color.FromArgb(104, 64, 165)))
                {
                    g.FillEllipse(b, 0, 0, size - 1, size - 1);
                }
                using (Font f = new Font("Segoe UI", size * 0.38f, FontStyle.Bold))
                using (SolidBrush textBrush = new SolidBrush(Color.White))
                {
                    StringFormat sf = new StringFormat
                    {
                        Alignment = StringAlignment.Center,
                        LineAlignment = StringAlignment.Center
                    };
                    g.DrawString("ALL", f, textBrush, new RectangleF(0, 0, size, size), sf);
                }
            }
            return bmp;
        }

        private void EnableDoubleBuffering(Control control)
        {
            typeof(Control).InvokeMember("DoubleBuffered",
                BindingFlags.SetProperty | BindingFlags.Instance | BindingFlags.NonPublic,
                null, control, new object[] { true });
        }

        private Image GetAvatar(string name, int size = 36)
        {
            if (string.IsNullOrEmpty(name)) name = "?";
            string key = $"{name}_{size}";
            if (!avatarCache.TryGetValue(key, out Image img))
            {
                img = AvatarHelper.GenerateAvatar(name, size);
                avatarCache[key] = img;
            }
            return img;
        }

        private void InitUserList()
        {
            onlineUsers.Clear();
            RefreshFilteredUserList();
        }

        #region Connection Logic
        //Client Core/Network: đăng xuất, đóng socket, dọn tài nguyên và trạng thái kết nối.
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
        //Client Core/Network: nhận ChatMessage từ server và điều phối tới giao diện.
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

        #region User List Management
        // GUI chính: lọc, cập nhật, chọn và vẽ danh sách người dùng.


        private void RefreshFilteredUserList()
        {
            string keyword = txtSearch.Text.Trim();

            isFilteringUsers = true;

            lstUsers.BeginUpdate();

            try
            {
                lstUsers.Items.Clear();
                lstUsers.Items.Add(new UserListItem
                {
                    Username = "Tất cả",
                    IsAll = true
                });

                foreach (UserListItem item in onlineUsers.Values)
                {
                    if (item.Username.IndexOf(
                        keyword,
                        StringComparison.OrdinalIgnoreCase) >= 0)
                    {
                        lstUsers.Items.Add(item);
                    }
                }

                int selectedIndex = currentRecipient == null ? 0 : -1;

                for (int i = 1; i < lstUsers.Items.Count; i++)
                {
                    UserListItem item = (UserListItem)lstUsers.Items[i];

                    if (string.Equals(
                        item.Username,
                        currentRecipient,
                        StringComparison.OrdinalIgnoreCase))
                    {
                        selectedIndex = i;
                        break;
                    }
                }

                lstUsers.SelectedIndex = selectedIndex;
            }
            finally
            {
                lstUsers.EndUpdate();
                isFilteringUsers = false;
            }
        }



        private void UpdateUserList(string[] users)
        {
            // Danh sách Online mới nhận từ Server
            var receivedUsers = new HashSet<string>(
                StringComparer.OrdinalIgnoreCase
            );

            if (users != null)
            {
                foreach (string user in users)
                {
                    string name = user?.Trim();

                    // Bỏ tên rỗng và tài khoản của chính mình
                    if (string.IsNullOrEmpty(name) ||
                        string.Equals(name, username,
                            StringComparison.OrdinalIgnoreCase))
                    {
                        continue;
                    }

                    receivedUsers.Add(name);

                    // Chỉ thêm tài khoản chưa tồn tại
                    if (!onlineUsers.ContainsKey(name))
                    {
                        onlineUsers.Add(name, new UserListItem
                        {
                            Username = name,
                            IsAll = false
                        });
                    }
                }
            }

            // Xóa người không còn Online theo danh sách Server
            foreach (string name in onlineUsers.Keys.ToList())
            {
                if (!receivedUsers.Contains(name))
                {
                    onlineUsers.Remove(name);
                }
            }

            bool recipientOffline =
                !string.IsNullOrEmpty(currentRecipient) &&
                !onlineUsers.ContainsKey(currentRecipient);

            RefreshFilteredUserList();


            if (recipientOffline)
            {
                SwitchToBroadcast();
            }
        }


        private void AddUserIfNotExists(string user)
        {
            string name = user?.Trim();

            if (string.IsNullOrEmpty(name) ||
                string.Equals(name, username, StringComparison.OrdinalIgnoreCase))
            {
                return;
            }

            if (onlineUsers.ContainsKey(name))
            {
                return;
            }

            onlineUsers.Add(name, new UserListItem
            {
                Username = name,
                IsAll = false
            });

            RefreshFilteredUserList();
        }


        private void RemoveUser(string user)
        {
            if (string.IsNullOrEmpty(user))
                return;

            onlineUsers.Remove(user);

            if (string.Equals(currentRecipient, user,
                StringComparison.OrdinalIgnoreCase))
            {
                SwitchToBroadcast();
            }
            RefreshFilteredUserList();
        }
        private void lstUsers_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (isFilteringUsers) return;

            if (lstUsers.SelectedItem is UserListItem item)
            {
                string nextRecipient = item.IsAll ? null : item.Username;

                if (!string.Equals(currentRecipient, nextRecipient,
                    StringComparison.OrdinalIgnoreCase))
                {
                    CancelReply();
                }

                if (item.IsAll)
                {
                    currentRecipient = null;
                    picChatHeaderAvatar.Image = allChannelAvatar;
                    lblCurrentChat.Text = "Kênh Công khai";
                    lblChatSub.Text = "Tất cả thành viên có thể đọc tin nhắn";
                }
                else
                {
                    currentRecipient = item.Username;
                    picChatHeaderAvatar.Image = GetAvatar(item.Username, 36);
                    lblCurrentChat.Text = item.Username;
                    lblChatSub.Text = "Trò chuyện riêng tư";
                    item.UnreadCount = 0;
                    lstUsers.Invalidate();
                }

                RefreshChatDisplay();
            }
        }

        private void SwitchToBroadcast()
        {
            if (lstUsers.Items.Count > 0)
            {
                lstUsers.SelectedIndex = 0;
            }
        }
        private void IncrementUnread(string senderName)
        {
            if (string.IsNullOrEmpty(senderName))
                return;

            if (onlineUsers.TryGetValue(senderName, out UserListItem item))
            {
                item.UnreadCount++;

                lstUsers.Invalidate();
            }
        }
        private void lstUsers_DrawItem(object sender, DrawItemEventArgs e)
        {
            if (e.Index < 0 || e.Index >= lstUsers.Items.Count) return;

            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

            UserListItem item = (UserListItem)lstUsers.Items[e.Index];
            bool isSelected = (e.State & DrawItemState.Selected) == DrawItemState.Selected;

            Color bgColor = isSelected ? Color.FromArgb(226, 214, 246) : Color.FromArgb(240, 242, 245);
            using (SolidBrush bgBrush = new SolidBrush(bgColor))
            {
                g.FillRectangle(bgBrush, e.Bounds);
            }

            if (isSelected)
            {
                using (SolidBrush barBrush = new SolidBrush(Color.FromArgb(76, 36, 140)))
                {
                    g.FillRectangle(barBrush, new Rectangle(e.Bounds.X, e.Bounds.Y + 4, 4, e.Bounds.Height - 8));
                }
            }

            Image avatar = item.IsAll ? allChannelAvatar : GetAvatar(item.Username, 36);
            int avatarX = e.Bounds.X + 12;
            int avatarY = e.Bounds.Y + (e.Bounds.Height - 36) / 2;
            if (avatar != null)
            {
                g.DrawImage(avatar, avatarX, avatarY, 36, 36);
            }

            if (!item.IsAll)
            {
                int dotX = avatarX + 24;
                int dotY = avatarY + 24;
                using (SolidBrush whiteBrush = new SolidBrush(Color.White))
                using (SolidBrush greenBrush = new SolidBrush(Color.FromArgb(52, 168, 83)))
                {
                    g.FillEllipse(whiteBrush, dotX - 1, dotY - 1, 12, 12);
                    g.FillEllipse(greenBrush, dotX, dotY, 10, 10);
                }
            }

            int textX = avatarX + 44;
            using (Font nameFont = new Font("Segoe UI", 9.5f, isSelected ? FontStyle.Bold : FontStyle.Regular))
            using (SolidBrush textBrush = new SolidBrush(Color.FromArgb(45, 24, 90)))
            {
                string name = item.IsAll ? "Tất cả (Phòng chung)" : item.Username;
                g.DrawString(name, nameFont, textBrush, textX, e.Bounds.Y + 8);
            }

            using (Font subFont = new Font("Segoe UI", 7.5f, FontStyle.Regular))
            using (SolidBrush subBrush = new SolidBrush(Color.FromArgb(110, 95, 140)))
            {
                string subText = item.IsAll ? "Kênh thảo luận chung" : "Đang hoạt động";
                g.DrawString(subText, subFont, subBrush, textX, e.Bounds.Y + 28);
            }

            if (item.UnreadCount > 0)
            {
                string unreadText = item.UnreadCount > 99 ? "99+" : item.UnreadCount.ToString();
                using (Font badgeFont = new Font("Segoe UI", 7.5f, FontStyle.Bold))
                {
                    Size badgeTextSize = TextRenderer.MeasureText(unreadText, badgeFont);
                    int badgeW = Math.Max(20, badgeTextSize.Width + 8);
                    int badgeH = 18;
                    int badgeX = e.Bounds.Right - badgeW - 12;
                    int badgeY = e.Bounds.Y + (e.Bounds.Height - badgeH) / 2;

                    using (GraphicsPath badgePath = CreateRoundedRectangle(new Rectangle(badgeX, badgeY, badgeW, badgeH), 9))
                    using (SolidBrush badgeBrush = new SolidBrush(Color.FromArgb(234, 67, 53)))
                    using (SolidBrush whiteBrush = new SolidBrush(Color.White))
                    {
                        g.FillPath(badgeBrush, badgePath);
                        StringFormat sf = new StringFormat
                        {
                            Alignment = StringAlignment.Center,
                            LineAlignment = StringAlignment.Center
                        };
                        g.DrawString(unreadText, badgeFont, whiteBrush, new RectangleF(badgeX, badgeY, badgeW, badgeH), sf);
                    }
                }
            }
        }

        private GraphicsPath CreateRoundedRectangle(Rectangle rect, int radius)
        {
            GraphicsPath path = new GraphicsPath();
            int d = radius * 2;
            path.AddArc(rect.X, rect.Y, d, d, 180, 90);
            path.AddArc(rect.Right - d, rect.Y, d, d, 270, 90);
            path.AddArc(rect.Right - d, rect.Bottom - d, d, d, 0, 90);
            path.AddArc(rect.X, rect.Bottom - d, d, d, 90, 90);
            path.CloseFigure();
            return path;
        }

        #endregion

        #region Messaging: Send, Reply, Forward, Display
        // Client Core/Network: btnSend_Click và luồng gửi tin nhắn cơ bản.
        //  Reply/Forward/Emoji: chỉ bổ sung nhánh Reply trong SendTextMessage;
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

        // GUI chính: lưu và hiển thị tin nhắn/bong bóng chat.
        private void AddSystemMessage(string content)
        {
            var sysMsg = new DisplayMessage
            {
                Id = Guid.NewGuid().ToString(),
                Sender = "Hệ thống",
                Content = content,
                Timestamp = DateTime.Now,
                IsSystem = true
            };
            AddMessageToChat(sysMsg);
        }

        private void AddMessageToChat(DisplayMessage msg)
        {
            if (!string.IsNullOrEmpty(msg.Id) && chatMessages.Any(m => m.Id == msg.Id))
                return;

            chatMessages.Add(msg);

            bool shouldDisplay = false;
            if (msg.IsSystem)
            {
                shouldDisplay = true;
            }
            else if (currentRecipient == null)
            {
                shouldDisplay = (msg.Recipient == null);
            }
            else
            {
                shouldDisplay = (string.Equals(msg.Sender, currentRecipient, StringComparison.OrdinalIgnoreCase) &&
                                 string.Equals(msg.Recipient, username, StringComparison.OrdinalIgnoreCase)) ||
                                (string.Equals(msg.Sender, username, StringComparison.OrdinalIgnoreCase) &&
                                 string.Equals(msg.Recipient, currentRecipient, StringComparison.OrdinalIgnoreCase));
            }

            if (shouldDisplay)
            {
                CreateAndAddBubble(msg);
            }
            else if (!msg.IsOwn && msg.Recipient != null && string.Equals(msg.Recipient, username, StringComparison.OrdinalIgnoreCase))
            {
                IncrementUnread(msg.Sender);
            }
        }

        private void RefreshChatDisplay()
        {
            pnlChatFlow.SuspendLayout();
            pnlChatFlow.Controls.Clear();

            IEnumerable<DisplayMessage> visibleMessages;
            if (currentRecipient == null)
            {
                visibleMessages = chatMessages.Where(m => m.IsSystem || m.Recipient == null);
            }
            else
            {
                visibleMessages = chatMessages.Where(m => m.IsSystem ||
                    (string.Equals(m.Sender, currentRecipient, StringComparison.OrdinalIgnoreCase) && string.Equals(m.Recipient, username, StringComparison.OrdinalIgnoreCase)) ||
                    (string.Equals(m.Sender, username, StringComparison.OrdinalIgnoreCase) && string.Equals(m.Recipient, currentRecipient, StringComparison.OrdinalIgnoreCase)));
            }

            foreach (var msg in visibleMessages)
            {
                CreateAndAddBubble(msg, false);
            }

            pnlChatFlow.ResumeLayout(true);
            ScrollToBottom();
        }

        private void CreateAndAddBubble(DisplayMessage msg, bool scroll = true)
        {
            Image avatar = msg.IsSystem ? null : GetAvatar(msg.Sender, 36);
            int bubbleWidth = GetCurrentBubbleWidth();

            var bubble = new MessageBubbleControl(msg, avatar, bubbleWidth)
            {
                Margin = new Padding(0, 2, 0, 2)
            };

            bubble.OnReplyClicked += (target) => StartReply(target);
            bubble.OnForwardClicked += (target) => PromptForward(target);

            pnlChatFlow.Controls.Add(bubble);

            if (scroll)
            {
                ScrollToBottom();
            }
        }

        private int GetCurrentBubbleWidth()
        {
            int w = pnlChatFlow.ClientSize.Width - 25;
            if (pnlChatFlow.VerticalScroll.Visible)
                w -= SystemInformation.VerticalScrollBarWidth;
            if (w < 200) w = 200;
            return w;
        }

        private void ScrollToBottom()
        {
            if (pnlChatFlow.Controls.Count > 0)
            {
                var lastControl = pnlChatFlow.Controls[pnlChatFlow.Controls.Count - 1];
                pnlChatFlow.ScrollControlIntoView(lastControl);
                pnlChatFlow.VerticalScroll.Value = pnlChatFlow.VerticalScroll.Maximum;
                pnlChatFlow.PerformLayout();
            }
        }

        private void pnlChatFlow_Resize(object sender, EventArgs e)
        {
            int newWidth = GetCurrentBubbleWidth();
            pnlChatFlow.SuspendLayout();
            foreach (Control c in pnlChatFlow.Controls)
            {
                if (c is MessageBubbleControl bubble)
                {
                    bubble.UpdateWidth(newWidth);
                }
            }
            pnlChatFlow.ResumeLayout(true);
        }

        private void StartReply(DisplayMessage msg)
        {
            replyingTo = new ChatMessage
            {
                Id = msg.Id,
                Sender = msg.Sender,
                Content = msg.Content
            };

            string preview = msg.Content;
            if (preview.Length > 50) preview = preview.Substring(0, 47) + "...";
            lblReplyPreviewText.Text = $"Đang trả lời {msg.Sender}: \"{preview}\"";
            pnlReplyPreview.Visible = true;
            txtMessage.Focus();
        }

        private void CancelReply()
        {
            replyingTo = null;
            pnlReplyPreview.Visible = false;
        }

        private void btnCancelReply_Click(object sender, EventArgs e)
        {
            CancelReply();
        }

        private void PromptForward(DisplayMessage msg)
        {
            if (client == null || !client.Connected)
            {
                MessageBox.Show("Bạn chưa kết nối tới máy chủ.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var recipients = new List<string> { "Tất cả (Phòng chung)" };

            recipients.AddRange(onlineUsers.Keys);

            using (var dlg = new ForwardForm(msg, recipients))
            {
                if (dlg.ShowDialog(this) == DialogResult.OK)
                {
                    string selected = dlg.SelectedRecipient;
                    string target = string.Equals(selected, "Tất cả (Phòng chung)", StringComparison.Ordinal) ? null : selected;
                    ForwardMessage(msg, target);
                }
            }
        }

        private async void ForwardMessage(DisplayMessage msg, string targetUser)
        {
            try
            {
                var fwdMsg = new ChatMessage
                {
                    Type = MessageType.ForwardMessage,
                    Sender = username,
                    Recipient = targetUser,
                    Content = msg.Content,
                    ForwardFrom = string.IsNullOrEmpty(msg.ForwardFrom) ? msg.Sender : msg.ForwardFrom,
                    Timestamp = DateTime.Now
                };

                if (chatConnection == null)
                    throw new InvalidOperationException("Kết nối tới máy chủ chưa sẵn sàng.");

                await chatConnection.SendAsync(fwdMsg, cts.Token);

                var displayMsg = new DisplayMessage
                {
                    Id = fwdMsg.Id,
                    Sender = username,
                    Recipient = targetUser,
                    Content = msg.Content,
                    ForwardFrom = fwdMsg.ForwardFrom,
                    Timestamp = fwdMsg.Timestamp,
                    IsOwn = true,
                    IsSystem = false
                };

                AddMessageToChat(displayMsg);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi chuyển tiếp tin nhắn: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        #endregion

        #region Emoji Picker
        //Reply/Forward/Emoji: mở bộ chọn và chèn emoji vào ô nhập.

        private void btnEmoji_Click(object sender, EventArgs e)
        {
            ShowEmojiPicker();
        }

        private void ShowEmojiPicker()
        {
            var picker = new EmojiPickerForm();
            picker.OnEmojiSelected += (emoji) =>
            {
                InsertEmoji(emoji);
            };
            picker.OnEnterPressed += async () =>
            {
                txtMessage.Focus();
                await SendTextMessage();
            };

            Point screenPt = btnEmoji.PointToScreen(new Point(0, -picker.Height - 4));
            if (screenPt.Y < 10)
            {
                screenPt = btnEmoji.PointToScreen(new Point(0, btnEmoji.Height + 4));
            }

            picker.Location = screenPt;
            picker.Show(this);
        }

        private void InsertEmoji(string emoji)
        {
            int sel = txtMessage.SelectionStart;
            txtMessage.Text = txtMessage.Text.Insert(sel, emoji);
            txtMessage.SelectionStart = sel + emoji.Length;
            txtMessage.SelectionLength = 0;
        }

        #endregion

        #region Input Handling & Form Lifecycle
        //GUI chính: phím Enter, điều hướng Settings/Chats, tìm kiếm và vòng đời form.

        private void txtMessage_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter && !e.Shift)
            {
                e.SuppressKeyPress = true;
                _ = SendTextMessage();
            }
        }

        private void pnlChatHeader_Paint(object sender, PaintEventArgs e)
        {
            using (var pen = new Pen(Color.FromArgb(230, 232, 235), 1))
            {
                e.Graphics.DrawLine(pen, 0, pnlChatHeader.Height - 1, pnlChatHeader.Width, pnlChatHeader.Height - 1);
            }
        }

        private void ClientForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            Disconnect();
        }

        #endregion
        private const int EM_SETCUEBANNER = 0x1501;

        [DllImport("user32.dll", CharSet = CharSet.Unicode)]
        private static extern IntPtr SendMessage(
            IntPtr hWnd,
            int msg,
            IntPtr wParam,
            string lParam);

        private bool replyWasVisible;
        private void btnSettings_Click(object sender, EventArgs e)
        {
            if (pnlSettingsView.Visible)
                return;

            replyWasVisible = pnlReplyPreview.Visible;

            pnlChatHeader.Visible = false;
            pnlChatFlow.Visible = false;
            pnlReplyPreview.Visible = false;
            pnlInput.Visible = false;

            UpdateSettingsInfo();

            pnlSettingsView.Visible = true;
            pnlSettingsView.BringToFront();

            splitContainerMain.Panel1Collapsed = true;
        }

        private void btnChats_Click(object sender, EventArgs e)
        {
            ShowChatView();
        }
        private void ShowChatView()
        {
            if (!pnlSettingsView.Visible)
                return;

            pnlSettingsView.Visible = false;

            splitContainerMain.Panel1Collapsed = false;

            pnlChatHeader.Visible = true;
            pnlChatFlow.Visible = true;
            pnlInput.Visible = true;

            pnlReplyPreview.Visible = replyWasVisible;
        }

        private void btnBackToChat_Click(object sender, EventArgs e)
        {
            ShowChatView();
        }

        private void UpdateSettingsInfo()
        {
            lblAccountUsernameValue.Text = lblOwnUsername.Text;
            lblAccountStatusValue.Text = lblOwnStatus.Text;
        }
        private void txtSearch_TextChanged(object sender, EventArgs e)
        {
            RefreshFilteredUserList();
        }
    }
}
