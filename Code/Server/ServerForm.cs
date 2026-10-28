using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using ChatShared.Helpers;
using ChatShared.Protocol;

namespace ChatServer
{
    public partial class ServerForm : Form
    {
        private const int MaxMessageLength = 65536;
        private const int ClientIdleTimeoutMilliseconds = 1800000; // 30 phút nhàn rỗi (tránh ngắt kết nối đột ngột khi chat)

        //Đối tượng TcpListener lắng nghe các yêu cầu kết nối TCP đến từ các client
        private TcpListener listener;

        //Nguồn token điều khiển hủy (cancellation) các tác vụ chạy nền khi dừng máy chủ
        private CancellationTokenSource cts;

        // Bảng băm an toàn luồng (ConcurrentDictionary) quản lý danh sách các client đang kết nối.
        // Key: Username (không phân biệt chữ hoa chữ thường nhờ StringComparer.OrdinalIgnoreCase).
        // Value: Đối tượng ClientInfo chứa thông tin phiên làm việc.
        private ConcurrentDictionary<string, ClientInfo> connectedClients = new ConcurrentDictionary<string, ClientInfo>(StringComparer.OrdinalIgnoreCase);

        //Cờ đánh dấu trạng thái máy chủ đang hoạt động hay đã dừng
        private bool isRunning = false;

        //Luồng ghi tệp nhật ký kiểm toán vào file server.log
        private StreamWriter logFileWriter;

        //Đối tượng khóa an toàn đa luồng khi ghi file log
        private readonly object logLock = new object();

        //Mốc thời gian máy chủ bắt đầu khởi động (dùng tính thời gian hoạt động Uptime)
        private DateTime serverStartTime;

        //Địa chỉ IP LAN nội bộ của máy chủ
        private string localIPAddress;

        //Bộ quản trị tài khoản người dùng (đăng ký, đăng nhập, bảo mật)
        private AccountManager accountManager;

        // Lớp đóng gói thông tin phiên làm việc của một máy khách (Client) kết nối vào máy chủ.
        public class ClientInfo
        {
            //Đối tượng TcpClient quản lý kết nối socket
            public TcpClient TcpClient { get; set; }

            //Luồng mạng NetworkStream phục vụ đọc/ghi dữ liệu nhị phân
            public NetworkStream Stream { get; set; }

            //Tên đăng nhập của người dùng
            public string Username { get; set; }

            //Chỉ số ảnh đại diện
            public int AvatarIndex { get; set; }

            //Mốc thời gian kết nối thành công
            public DateTime ConnectedAt { get; set; }

            //Token hủy riêng cho từng kết nối client
            public CancellationTokenSource ClientCts { get; set; }

            // Khóa nhị phân SemaphoreSlim(1,1) độc quyền trên từng kết nối client.
            // Ngăn chặn hiện tượng xung đột dữ liệu (data corruption) khi nhiều luồng cùng ghi vào NetworkStream đồng thời.
            public SemaphoreSlim SendLock { get; } = new SemaphoreSlim(1, 1);

            //Địa chỉ IP và cổng từ xa của client (Remote EndPoint)
            public IPEndPoint RemoteEndPoint { get; set; }
        }

        public ServerForm()
        {
            InitializeComponent();
        }

        // Sự kiện khởi tạo Form máy chủ: Tự động phát hiện địa chỉ IP LAN cục bộ và hiển thị lên giao diện.
        private void ServerForm_Load(object sender, EventArgs e)
        {
            accountManager = new AccountManager();
            localIPAddress = GetLocalIPAddress();
            lblIPValue.Text = localIPAddress;
            tsLocalIP.Text = $"Địa chỉ IP: {localIPAddress}";
            UpdateUIState(false);
            Log("SYSTEM", $"Giao diện máy chủ đã sẵn sàng. Đã nạp {accountManager.TotalAccounts} tài khoản đã đăng ký.");
        }

        #region Server Lifecycle Management

        // Starts the TCP listener and begins accepting incoming connections
        public async Task StartServer()
        {
            if (isRunning) return;

            if (!int.TryParse(txtPort.Text.Trim(), out int port) || port < 1 || port > 65535)
            {
                MessageBox.Show("Vui lòng nhập số cổng hợp lệ (1 - 65535).", "Cổng không hợp lệ", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtPort.Focus();
                return;
            }

            try
            {
                cts = new CancellationTokenSource();
                listener = new TcpListener(IPAddress.Any, port);
                listener.Start();

                isRunning = true;
                serverStartTime = DateTime.Now;
                uptimeTimer.Start();

                UpdateUIState(true);
                Log("SERVER", $"Máy chủ đã khởi động thành công trên cổng {port}. Đang chờ kết nối...");

                _ = AcceptClientsLoop(cts.Token);
            }
            catch (SocketException sex)
            {
                isRunning = false;
                UpdateUIState(false);
                MessageBox.Show($"Không thể mở cổng {port}. Cổng có thể đang bị ứng dụng khác chiếm dụng.\nChi tiết: {sex.Message}", "Lỗi khởi động máy chủ", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Log("ERROR", $"Khởi động thất bại trên cổng {port}: {sex.Message}");
            }
            catch (Exception ex)
            {
                isRunning = false;
                UpdateUIState(false);
                MessageBox.Show($"Đã xảy ra lỗi khi khởi động máy chủ:\n{ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Log("ERROR", $"Lỗi khởi động máy chủ: {ex.Message}");
            }
        }

        // Stops the TCP listener and gracefully disconnects all connected clients
        public async Task StopServer()
        {
            if (!isRunning) return;

            isRunning = false;
            uptimeTimer.Stop();

            Log("SERVER", "Đang dừng máy chủ...");

            try
            {
                cts?.Cancel();
            }
            catch { }

            try
            {
                listener?.Stop();
            }
            catch { }

            // Notify and disconnect all connected clients
            var clients = connectedClients.Values.ToList();
            if (clients.Count > 0)
            {
                var shutdownNotice = new ChatMessage
                {
                    Type = MessageType.ServerMessage,
                    Sender = "[Máy chủ]",
                    Content = "Máy chủ đang tắt. Kết nối sẽ bị ngắt.",
                    Timestamp = DateTime.Now
                };

                var disconnectTasks = clients.Select(async client =>
                {
                    try
                    {
                        await SendToClient(client, shutdownNotice);
                    }
                    catch { }
                    finally
                    {
                        DisconnectClient(client, "Máy chủ tắt");
                    }
                });

                await Task.WhenAll(disconnectTasks);
            }

            connectedClients.Clear();
            UpdateClientListUI();
            UpdateUIState(false);

            Log("SERVER", "Máy chủ đã dừng hoàn toàn.");
        }

        // Async loop accepting incoming TCP connections
        private async Task AcceptClientsLoop(CancellationToken ct)
        {
            while (!ct.IsCancellationRequested && isRunning)
            {
                try
                {
                    TcpClient tcpClient = await listener.AcceptTcpClientAsync();
                    _ = Task.Run(() => HandleClient(tcpClient, ct), ct);
                }
                catch (ObjectDisposedException)
                {
                    break;
                }
                catch (SocketException sex)
                {
                    if (!isRunning || ct.IsCancellationRequested) break;
                    Log("WARN", $"Lỗi socket khi tiếp nhận kết nối: {sex.Message}");
                    try { await Task.Delay(500, ct); } catch { break; }
                }
                catch (Exception ex)
                {
                    if (!isRunning || ct.IsCancellationRequested) break;
                    Log("ERROR", $"Lỗi tiếp nhận kết nối: {ex.Message}");
                    try { await Task.Delay(500, ct); } catch { break; }
                }
            }
        }

        #endregion

        #region Client Handling & Message Processing

        // Handles the full lifecycle of a connected client
        public async Task HandleClient(TcpClient tcpClient, CancellationToken serverCt)
        {
            var clientCts = CancellationTokenSource.CreateLinkedTokenSource(serverCt);
            IPEndPoint endpoint = null;
            try
            {
                endpoint = tcpClient.Client.RemoteEndPoint as IPEndPoint;
            }
            catch { }

            Log("CONNECT", $"Kết nối mới từ {endpoint}");

            ClientInfo clientInfo = null;
            string username = null;

            try
            {
                // Không ép timeout đồng bộ ở tầng socket (0 = vô hạn), tránh ngắt kết nối async sau 30s
                tcpClient.ReceiveTimeout = 0;
                tcpClient.SendTimeout = 0;
                tcpClient.NoDelay = true;

                var stream = tcpClient.GetStream();

                // 1. Vòng lặp xác thực (Authentication Loop): Hỗ trợ Đăng ký (Register) và Đăng nhập (Login)
                bool isAuthenticated = false;
                while (!clientCts.IsCancellationRequested && tcpClient.Connected && isRunning && !isAuthenticated)
                {
                    ChatMessage authMsg;
                    try
                    {
                        using (var timeoutCts = new CancellationTokenSource(30000))
                        using (var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(clientCts.Token, timeoutCts.Token))
                        {
                            authMsg = await MessageSerializer.ReceiveMessageAsync(stream, linkedCts.Token);
                        }
                    }
                    catch (OperationCanceledException)
                    {
                        Log("TIMEOUT", $"Quá thời gian chờ xác thực từ {endpoint}");
                        return;
                    }

                    if (authMsg == null)
                    {
                        Log("DISCONNECT", $"Kết nối đóng trước khi hoàn tất xác thực từ {endpoint}");
                        return;
                    }

                    if (authMsg.Type == MessageType.Register)
                    {
                        string regUser = authMsg.Sender?.Trim();
                        string regPass = authMsg.Password;
                        Log("REGISTER_REQ", $"Yêu cầu đăng ký tài khoản '{regUser}' từ {endpoint}");

                        if (accountManager.Register(regUser, regPass, authMsg.AvatarIndex, out string regError))
                        {
                            Log("REGISTER", $"Đăng ký thành công tài khoản '{regUser}' từ {endpoint}.");
                            var regSuccess = new ChatMessage
                            {
                                Type = MessageType.RegisterResult,
                                Success = true,
                                Content = "Đăng ký tài khoản thành công! Bạn có thể đăng nhập ngay.",
                                Sender = regUser,
                                Timestamp = DateTime.Now
                            };
                            await MessageSerializer.SendMessageAsync(stream, regSuccess, clientCts.Token);
                        }
                        else
                        {
                            Log("REGISTER_REJECT", $"Từ chối đăng ký '{regUser}' từ {endpoint}: {regError}");
                            var regFail = new ChatMessage
                            {
                                Type = MessageType.RegisterResult,
                                Success = false,
                                Content = regError,
                                Sender = regUser,
                                Timestamp = DateTime.Now
                            };
                            await MessageSerializer.SendMessageAsync(stream, regFail, clientCts.Token);
                        }
                        continue;
                    }
                    else if (authMsg.Type == MessageType.Login)
                    {
                        string reqUser = authMsg.Sender?.Trim();
                        string reqPass = authMsg.Password;

                        if (!accountManager.VerifyLogin(reqUser, reqPass, out var userAccount, out string loginError))
                        {
                            Log("LOGIN_REJECT", $"Từ chối đăng nhập '{reqUser}' từ {endpoint}: {loginError}");
                            try
                            {
                                var rejectMsg = new ChatMessage
                                {
                                    Type = MessageType.LoginResult,
                                    Success = false,
                                    Content = loginError,
                                    Timestamp = DateTime.Now
                                };
                                await MessageSerializer.SendMessageAsync(stream, rejectMsg, clientCts.Token);
                            }
                            catch { }
                            continue;
                        }

                        // Check if account is already logged in / online
                        if (connectedClients.ContainsKey(reqUser))
                        {
                            Log("LOGIN_REJECT", $"Tài khoản '{reqUser}' hiện đang trực tuyến tại một phiên khác.");
                            try
                            {
                                var rejectMsg = new ChatMessage
                                {
                                    Type = MessageType.LoginResult,
                                    Success = false,
                                    Content = "Tài khoản này hiện đang trực tuyến tại một phiên khác.",
                                    Timestamp = DateTime.Now
                                };
                                await MessageSerializer.SendMessageAsync(stream, rejectMsg, clientCts.Token);
                            }
                            catch { }
                            continue;
                        }

                        username = reqUser;
                        clientInfo = new ClientInfo
                        {
                            TcpClient = tcpClient,
                            Stream = stream,
                            Username = username,
                            AvatarIndex = userAccount.AvatarIndex,
                            ConnectedAt = DateTime.Now,
                            ClientCts = clientCts,
                            RemoteEndPoint = endpoint
                        };

                        if (!connectedClients.TryAdd(username, clientInfo))
                        {
                            Log("LOGIN_REJECT", $"Xung đột phiên làm việc cho tài khoản '{username}'.");
                            try
                            {
                                var rejectMsg = new ChatMessage
                                {
                                    Type = MessageType.LoginResult,
                                    Success = false,
                                    Content = "Tài khoản hiện đang được đăng nhập. Vui lòng thử lại.",
                                    Timestamp = DateTime.Now
                                };
                                await MessageSerializer.SendMessageAsync(stream, rejectMsg, clientCts.Token);
                            }
                            catch { }
                            continue;
                        }

                        Log("LOGIN", $"Người dùng '{username}' ({endpoint}) đã đăng nhập thành công.");

                        // Send LoginResult Success with current user list
                        var loginSuccess = new ChatMessage
                        {
                            Type = MessageType.LoginResult,
                            Success = true,
                            Content = "Đăng nhập thành công!",
                            UserList = connectedClients.Keys.ToArray(),
                            AvatarIndex = clientInfo.AvatarIndex,
                            Timestamp = DateTime.Now
                        };
                        await SendToClient(clientInfo, loginSuccess);

                        // Broadcast UserJoined notification to others
                        var joinedMsg = new ChatMessage
                        {
                            Type = MessageType.UserJoined,
                            Sender = username,
                            AvatarIndex = clientInfo.AvatarIndex,
                            Content = $"{username} đã tham gia phòng chat.",
                            Timestamp = DateTime.Now
                        };
                        await BroadcastMessage(joinedMsg, excludeUser: username);

                        // Broadcast updated user list to everyone
                        await BroadcastUserList();

                        // Update UI client list
                        UpdateClientListUI();

                        isAuthenticated = true;
                        break;
                    }
                    else
                    {
                        Log("AUTH_INVALID", $"Thông điệp không hợp lệ khi chưa xác thực từ {endpoint} (Loại: {authMsg.Type})");
                        try
                        {
                            var err = new ChatMessage
                            {
                                Type = MessageType.Error,
                                Content = "Bạn phải đăng nhập hoặc đăng ký tài khoản trước khi thực hiện các hành động khác.",
                                Timestamp = DateTime.Now
                            };
                            await MessageSerializer.SendMessageAsync(stream, err, clientCts.Token);
                        }
                        catch { }
                    }
                }

                if (!isAuthenticated || clientInfo == null)
                {
                    return;
                }

                // 2. Receive and process messages loop
                while (!clientCts.IsCancellationRequested && tcpClient.Connected && isRunning)
                {
                    ChatMessage msg;
                    try
                    {
                        using (var idleTimeoutCts = new CancellationTokenSource(ClientIdleTimeoutMilliseconds))
                        using (var readCts = CancellationTokenSource.CreateLinkedTokenSource(clientCts.Token, idleTimeoutCts.Token))
                        {
                            msg = await MessageSerializer.ReceiveMessageAsync(stream, readCts.Token);
                        }
                    }
                    catch (OperationCanceledException) when (!clientCts.IsCancellationRequested)
                    {
                        Log("IDLE_TIMEOUT", $"Ngắt kết nối '{username}' vì không có dữ liệu trong {ClientIdleTimeoutMilliseconds / 60000} phút.");
                        break;
                    }
                    catch (InvalidDataException ide)
                    {
                        Log("DATA_ERROR", $"Dữ liệu sai định dạng từ '{username}': {ide.Message}");
                        break;
                    }
                    catch (IOException)
                    {
                        break;
                    }
                    catch (SocketException)
                    {
                        break;
                    }

                    if (msg == null)
                    {
                        // Disconnected by peer
                        break;
                    }

                    // Authentication and server-only messages are invalid after login.
                    if (msg.Type == MessageType.Login || msg.Type == MessageType.Register ||
                        msg.Type == MessageType.LoginResult || msg.Type == MessageType.RegisterResult ||
                        msg.Type == MessageType.UserList || msg.Type == MessageType.UserJoined ||
                        msg.Type == MessageType.UserLeft || msg.Type == MessageType.ServerMessage ||
                        msg.Type == MessageType.MessageDelivered || msg.Type == MessageType.Error)
                    {
                        await SendProtocolError(clientInfo, "Loại thông điệp không hợp lệ từ client đã đăng nhập.");
                        continue;
                    }

                    // Enforce authenticated sender identity to prevent impersonation
                    msg.Sender = username;
                    msg.Timestamp = DateTime.Now;

                    // Content length validation (maximum 64KB)
                    if (msg.Content != null && msg.Content.Length > MaxMessageLength)
                    {
                        Log("VALIDATION", $"Tin nhắn từ '{username}' vượt quá giới hạn 64KB.");
                        await SendProtocolError(clientInfo, "Nội dung tin nhắn vượt quá dung lượng tối đa cho phép (64KB).");
                        continue;
                    }

                    if (RequiresContent(msg.Type) && string.IsNullOrWhiteSpace(msg.Content))
                    {
                        Log("VALIDATION", $"Tin nhắn rỗng từ '{username}'.");
                        await SendProtocolError(clientInfo, "Nội dung tin nhắn không được để trống.");
                        continue;
                    }

                    await ProcessMessage(clientInfo, msg);
                }
            }
            catch (OperationCanceledException) { }
            catch (Exception ex)
            {
                Log("CLIENT_ERROR", $"Lỗi từ kết nối '{username ?? endpoint?.ToString() ?? "N/A"}': {ex.Message}");
            }
            finally
            {
                if (clientInfo != null)
                {
                    DisconnectClient(clientInfo);
                }
                else
                {
                    try { clientCts?.Cancel(); } catch { }
                    try { tcpClient?.Close(); } catch { }
                    Log("DISCONNECT", $"Kết nối từ {endpoint} đã đóng.");
                }
            }
        }

        private static bool RequiresContent(MessageType type)
        {
            return type == MessageType.TextMessage || type == MessageType.PrivateMessage ||
                   type == MessageType.ReplyMessage || type == MessageType.ForwardMessage ||
                   type == MessageType.EmojiMessage;
        }

        private Task SendProtocolError(ClientInfo client, string message)
        {
            return SendToClient(client, new ChatMessage
            {
                Type = MessageType.Error,
                Content = message,
                Timestamp = DateTime.Now
            });
        }

        /// <summary>
        /// Validates username rules: non-empty, length 2-25, no special symbols, unique, non-reserved
        /// </summary>
        private string ValidateUsername(string username)
        {
            if (string.IsNullOrWhiteSpace(username))
                return "Tên đăng nhập không được để trống.";

            username = username.Trim();
            if (username.Length < 2)
                return "Tên đăng nhập phải có ít nhất 2 ký tự.";

            if (username.Length > 25)
                return "Tên đăng nhập không được dài quá 25 ký tự.";

            string[] reserved = { "server", "máy chủ", "may chu", "admin", "quản trị viên", "quan tri vien", "hệ thống", "he thong", "all", "tất cả", "tat ca" };
            if (reserved.Any(r => r.Equals(username, StringComparison.OrdinalIgnoreCase)))
                return "Tên đăng nhập này là từ khóa bảo lưu của hệ thống.";

            foreach (char c in username)
            {
                if (!char.IsLetterOrDigit(c) && c != ' ' && c != '_' && c != '-')
                    return "Tên đăng nhập chỉ được chứa chữ cái, chữ số, khoảng trắng, gạch dưới (_) hoặc gạch ngang (-).";
            }

            if (connectedClients.ContainsKey(username))
                return "Tên đăng nhập đã được sử dụng. Vui lòng chọn tên khác.";

            return null;
        }

        /// <summary>
        /// Routes messages based on MessageType
        /// </summary>
        public async Task ProcessMessage(ClientInfo sender, ChatMessage msg)
        {
            switch (msg.Type)
            {
                case MessageType.TextMessage:
                case MessageType.EmojiMessage:
                    if (string.IsNullOrWhiteSpace(msg.Recipient))
                    {
                        string tag = msg.Type == MessageType.EmojiMessage ? "EMOJI" : "CHAT";
                        Log(tag, $"Người gửi '{sender.Username}' gửi tin công khai ({msg.Content.Length} ký tự).");
                        await BroadcastMessage(msg);
                    }
                    else
                    {
                        await RoutePrivateMessage(sender, msg);
                    }
                    break;

                case MessageType.PrivateMessage:
                    await RoutePrivateMessage(sender, msg);
                    break;

                case MessageType.ReplyMessage:
                    if (string.IsNullOrWhiteSpace(msg.Recipient))
                    {
                        Log("REPLY", $"'{sender.Username}' trả lời '{msg.ReplyToSender}' trong kênh công khai ({msg.Content.Length} ký tự).");
                        await BroadcastMessage(msg);
                    }
                    else
                    {
                        Log("REPLY_PRIVATE", $"'{sender.Username}' trả lời riêng '{msg.Recipient}' ({msg.Content.Length} ký tự).");
                        await RoutePrivateMessage(sender, msg);
                    }
                    break;

                case MessageType.ForwardMessage:
                    if (string.IsNullOrWhiteSpace(msg.Recipient))
                    {
                        Log("FORWARD", $"'{sender.Username}' chuyển tiếp từ '{msg.ForwardFrom}' trong kênh công khai ({msg.Content.Length} ký tự).");
                        await BroadcastMessage(msg);
                    }
                    else
                    {
                        Log("FORWARD_PRIVATE", $"'{sender.Username}' chuyển tiếp tới '{msg.Recipient}' từ '{msg.ForwardFrom}' ({msg.Content.Length} ký tự).");
                        await RoutePrivateMessage(sender, msg);
                    }
                    break;

                case MessageType.RequestUserList:
                    Log("USER_LIST_REQ", $"Người dùng '{sender.Username}' yêu cầu danh sách trực tuyến.");
                    var listMsg = new ChatMessage
                    {
                        Type = MessageType.UserList,
                        UserList = connectedClients.Keys.ToArray(),
                        Timestamp = DateTime.Now
                    };
                    await SendToClient(sender, listMsg);
                    break;

                case MessageType.Logout:
                    Log("LOGOUT", $"Người dùng '{sender.Username}' đã đăng xuất.");
                    DisconnectClient(sender, "Người dùng đã đăng xuất");
                    break;

                default:
                    Log("UNKNOWN_MSG", $"Nhận thông điệp không xác định ({msg.Type}) từ '{sender.Username}'");
                    await SendProtocolError(sender, "Loại thông điệp không được hỗ trợ.");
                    break;
            }
        }

        /// <summary>
        /// Routes a direct message to a specific recipient and confirms delivery
        /// </summary>
        private async Task RoutePrivateMessage(ClientInfo sender, ChatMessage msg)
        {
            if (string.IsNullOrWhiteSpace(msg.Recipient))
            {
                await SendToClient(sender, new ChatMessage
                {
                    Type = MessageType.Error,
                    Content = "Chưa chỉ định người nhận cho tin nhắn riêng.",
                    Timestamp = DateTime.Now
                });
                return;
            }

            if (connectedClients.TryGetValue(msg.Recipient, out var recipientClient))
            {
                Log("PRIVATE", $"Chuyển tin riêng từ '{sender.Username}' tới '{msg.Recipient}' ({msg.Content.Length} ký tự).");

                // Deliver to target recipient
                await SendToClient(recipientClient, msg);

                // Echo back to sender for chat history consistency
                await SendToClient(sender, msg);

                // Send delivery confirmation
                var confirmMsg = new ChatMessage
                {
                    Type = MessageType.MessageDelivered,
                    Id = msg.Id,
                    Recipient = msg.Recipient,
                    Timestamp = DateTime.Now
                };
                await SendToClient(sender, confirmMsg);
            }
            else
            {
                Log("ROUTE_FAIL", $"Không tìm thấy người nhận '{msg.Recipient}' từ '{sender.Username}'");
                await SendToClient(sender, new ChatMessage
                {
                    Type = MessageType.Error,
                    Content = $"Người dùng '{msg.Recipient}' hiện không trực tuyến hoặc không tồn tại.",
                    Timestamp = DateTime.Now
                });
            }
        }

        /// <summary>
        /// Broadcasts message to all connected clients, optionally excluding a specific user
        /// </summary>
        public async Task BroadcastMessage(ChatMessage msg, string excludeUser = null)
        {
            var clients = connectedClients.Values.ToList();
            var tasks = new List<Task>();

            foreach (var client in clients)
            {
                if (excludeUser != null && client.Username.Equals(excludeUser, StringComparison.OrdinalIgnoreCase))
                    continue;

                tasks.Add(SendToClient(client, msg));
            }

            await Task.WhenAll(tasks);
        }

        /// <summary>
        /// Sends message to a specific client with synchronization lock
        /// </summary>
        public async Task<bool> SendToClient(ClientInfo client, ChatMessage msg)
        {
            if (client == null || client.Stream == null || !client.TcpClient.Connected)
                return false;

            bool lockTaken = false;
            try
            {
                lockTaken = await client.SendLock.WaitAsync(5000);
                if (!lockTaken)
                {
                    Log("SEND_TIMEOUT", $"Không thể gửi tới '{client.Username}' do socket bận.");
                    return false;
                }

                await MessageSerializer.SendMessageAsync(client.Stream, msg, client.ClientCts.Token);
                return true;
            }
            catch (Exception ex)
            {
                Log("SEND_ERROR", $"Lỗi gửi tin nhắn tới '{client.Username}': {ex.Message}");
                _ = Task.Run(() => DisconnectClient(client, "Lỗi kết nối khi gửi dữ liệu"));
                return false;
            }
            finally
            {
                if (lockTaken)
                {
                    client.SendLock.Release();
                }
            }
        }

        /// <summary>
        /// Broadcasts updated user list to all connected clients
        /// </summary>
        public async Task BroadcastUserList()
        {
            var listMsg = new ChatMessage
            {
                Type = MessageType.UserList,
                UserList = connectedClients.Keys.ToArray(),
                Timestamp = DateTime.Now
            };

            await BroadcastMessage(listMsg);
        }

        /// <summary>
        /// Safely removes and disconnects a client, broadcasting notifications
        /// </summary>
        public void DisconnectClient(ClientInfo client, string reason = null)
        {
            if (client == null) return;

            bool wasRegistered = false;
            if (!string.IsNullOrEmpty(client.Username))
            {
                wasRegistered = connectedClients.TryRemove(client.Username, out _);
            }

            try { client.ClientCts?.Cancel(); } catch { }
            try { client.Stream?.Close(); } catch { }
            try { client.TcpClient?.Close(); } catch { }

            if (wasRegistered && !string.IsNullOrEmpty(client.Username))
            {
                string detail = reason != null ? $"Lý do: {reason}" : "Ngắt kết nối";
                Log("DISCONNECT", $"Người dùng '{client.Username}' đã ngắt kết nối ({detail}).");

                // Broadcast UserLeft notification
                var leftMsg = new ChatMessage
                {
                    Type = MessageType.UserLeft,
                    Sender = client.Username,
                    Content = $"{client.Username} đã rời phòng chat.",
                    Timestamp = DateTime.Now
                };
                _ = BroadcastMessage(leftMsg);

                // Broadcast updated user list
                _ = BroadcastUserList();

                // Update UI list
                UpdateClientListUI();
            }
            else
            {
                Log("DISCONNECT", $"Đã đóng kết nối từ {client.RemoteEndPoint}.");
            }
        }

        /// <summary>
        /// Kicks a client by username with a specified reason
        /// </summary>
        public async Task KickClient(string username, string reason = "Bị ngắt kết nối bởi máy chủ")
        {
            if (string.IsNullOrWhiteSpace(username)) return;

            if (connectedClients.TryGetValue(username, out var client))
            {
                Log("KICK", $"Máy chủ ngắt kết nối người dùng '{username}'. Lý do: {reason}");

                try
                {
                    var kickNotice = new ChatMessage
                    {
                        Type = MessageType.ServerMessage,
                        Sender = "[Máy chủ]",
                        Content = $"Bạn đã bị ngắt kết nối bởi máy chủ. Lý do: {reason}",
                        Timestamp = DateTime.Now
                    };
                    await SendToClient(client, kickNotice);
                    await Task.Delay(200);
                }
                catch { }

                DisconnectClient(client, reason);
            }
        }

        #endregion

        #region Logging & UI Updates

        // Thread-safe logging to both TextBox and server.log file
        // Format: [yyyy-MM-dd HH:mm:ss] EVENT - details
        public void Log(string eventType, string message)
        {
            string timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            string formatted = $"[{timestamp}] {eventType} - {message}";

            // 1. Thread-safe write to server.log
            lock (logLock)
            {
                try
                {
                    if (logFileWriter == null)
                    {
                        string logPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "server.log");
                        logFileWriter = new StreamWriter(new FileStream(logPath, FileMode.Append, FileAccess.Write, FileShare.ReadWrite), Encoding.UTF8)
                        {
                            AutoFlush = true
                        };
                    }
                    logFileWriter.WriteLine(formatted);
                }
                catch { }
            }

            // 2. Thread-safe UI update
            if (txtLog == null || txtLog.IsDisposed) return;

            if (txtLog.InvokeRequired)
            {
                try
                {
                    txtLog.BeginInvoke(new Action(() => AppendLogLine(formatted)));
                }
                catch { }
            }
            else
            {
                AppendLogLine(formatted);
            }
        }

        private void AppendLogLine(string line)
        {
            if (txtLog.IsDisposed) return;

            // Trim buffer when exceeding 300KB to prevent memory exhaustion
            if (txtLog.TextLength > 300000)
            {
                txtLog.Select(0, 100000);
                txtLog.SelectedText = "";
            }

            txtLog.AppendText(line + Environment.NewLine);
            txtLog.SelectionStart = txtLog.TextLength;
            txtLog.ScrollToCaret();
        }

        // Updates the ListView showing connected clients with their avatars, endpoints, and connected times
        public void UpdateClientListUI()
        {
            if (this.IsDisposed || !this.IsHandleCreated) return;

            if (this.InvokeRequired)
            {
                try
                {
                    this.BeginInvoke(new Action(UpdateClientListUIInternal));
                }
                catch { }
            }
            else
            {
                UpdateClientListUIInternal();
            }
        }

        private void UpdateClientListUIInternal()
        {
            if (lvClients.IsDisposed) return;

            lvClients.BeginUpdate();
            try
            {
                var clients = connectedClients.Values.ToList();

                // Populate avatar icons into ImageList
                foreach (var client in clients)
                {
                    if (!imgListAvatars.Images.ContainsKey(client.Username))
                    {
                        var avatar = AvatarHelper.GenerateAvatar(client.Username, 32);
                        imgListAvatars.Images.Add(client.Username, avatar);
                    }
                }

                string selectedUser = lvClients.SelectedItems.Count > 0 ? lvClients.SelectedItems[0].Text : null;

                lvClients.Items.Clear();
                foreach (var client in clients)
                {
                    var item = new ListViewItem(client.Username, client.Username);
                    item.SubItems.Add(client.RemoteEndPoint?.ToString() ?? "N/A");
                    item.SubItems.Add(client.ConnectedAt.ToString("HH:mm:ss"));
                    item.Tag = client;

                    if (client.Username == selectedUser)
                    {
                        item.Selected = true;
                    }

                    lvClients.Items.Add(item);
                }

                grpClients.Text = $"Danh sách kết nối ({clients.Count})";
                tsClientCount.Text = $"Đang kết nối: {clients.Count} người dùng";
                btnKick.Enabled = isRunning && lvClients.SelectedItems.Count > 0;
            }
            finally
            {
                lvClients.EndUpdate();
            }
        }

        private void UpdateUIState(bool running)
        {
            btnStart.Enabled = !running;
            btnStop.Enabled = running;
            txtPort.Enabled = !running;

            if (running)
            {
                lblStatusBadge.Text = "HOẠT ĐỘNG";
                lblStatusBadge.BackColor = Color.FromArgb(16, 185, 129); // Green
                tsStatus.Text = "Trạng thái: Đang hoạt động";
                tsStatus.ForeColor = Color.FromArgb(16, 185, 129);
            }
            else
            {
                lblStatusBadge.Text = "ĐÃ DỪNG";
                lblStatusBadge.BackColor = Color.FromArgb(100, 116, 139); // Gray
                tsStatus.Text = "Trạng thái: Đã dừng";
                tsStatus.ForeColor = Color.FromArgb(220, 38, 38);
                btnKick.Enabled = false;
            }
        }

        private string GetLocalIPAddress()
        {
            try
            {
                var host = Dns.GetHostEntry(Dns.GetHostName());
                foreach (var ip in host.AddressList)
                {
                    if (ip.AddressFamily == AddressFamily.InterNetwork && !IPAddress.IsLoopback(ip))
                    {
                        return ip.ToString();
                    }
                }
                return "127.0.0.1";
            }
            catch
            {
                return "127.0.0.1";
            }
        }

        #endregion

        #region Event Handlers

        private async void btnStart_Click(object sender, EventArgs e)
        {
            await StartServer();
        }

        private async void btnStop_Click(object sender, EventArgs e)
        {
            await StopServer();
        }

        private async void btnBroadcast_Click(object sender, EventArgs e)
        {
            await BroadcastServerMessageAsync();
        }

        private async void txtBroadcast_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true;
                await BroadcastServerMessageAsync();
            }
        }

        private async Task BroadcastServerMessageAsync()
        {
            string content = txtBroadcast.Text.Trim();
            if (string.IsNullOrEmpty(content)) return;

            if (!isRunning)
            {
                MessageBox.Show("Máy chủ hiện chưa hoạt động. Hãy khởi động máy chủ trước khi gửi thông báo.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            txtBroadcast.Clear();

            var msg = new ChatMessage
            {
                Type = MessageType.ServerMessage,
                Sender = "[Máy chủ]",
                Content = content,
                Timestamp = DateTime.Now
            };

            Log("BROADCAST", $"[Máy chủ]: {content}");
            await BroadcastMessage(msg);
        }

        private async void btnKick_Click(object sender, EventArgs e)
        {
            if (lvClients.SelectedItems.Count == 0)
            {
                MessageBox.Show("Vui lòng chọn người dùng cần ngắt kết nối từ danh sách.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            string username = lvClients.SelectedItems[0].Text;
            DialogResult result = MessageBox.Show(
                $"Bạn có chắc chắn muốn ngắt kết nối người dùng '{username}' khỏi hệ thống?",
                "Xác nhận ngắt kết nối",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                await KickClient(username, "Bị ngắt kết nối bởi quản trị viên máy chủ");
            }
        }

        private void btnClearLog_Click(object sender, EventArgs e)
        {
            txtLog.Clear();
        }

        private void lvClients_SelectedIndexChanged(object sender, EventArgs e)
        {
            btnKick.Enabled = isRunning && lvClients.SelectedItems.Count > 0;
        }

        private void uptimeTimer_Tick(object sender, EventArgs e)
        {
            if (isRunning)
            {
                TimeSpan elapsed = DateTime.Now - serverStartTime;
                tsUptime.Text = $"Thời gian hoạt động: {elapsed:hh\\:mm\\:ss}";
            }
            else
            {
                tsUptime.Text = "Thời gian hoạt động: 00:00:00";
            }
        }

        private async void ServerForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (isRunning)
            {
                e.Cancel = true;
                await StopServer();
                isRunning = false;
                this.FormClosing -= ServerForm_FormClosing;
                this.Close();
            }
            else
            {
                lock (logLock)
                {
                    try
                    {
                        logFileWriter?.Flush();
                        logFileWriter?.Close();
                        logFileWriter = null;
                    }
                    catch { }
                }
            }
        }

        #endregion
    }
}
