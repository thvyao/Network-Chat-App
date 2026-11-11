using ChatShared.Protocol;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.ConstrainedExecution;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ChatClient
{
    public partial class LoginForm : Form
    {
        // Thông tin phiên đăng nhập sau khi Server xác thực thành công
        public string AuthenticatedUser { get; private set; }
        public TcpClient TcpClient { get; private set; }
        public NetworkStream Stream { get; private set; }
        public ChatMessage InitialMessage { get; private set; }
        public string ServerIP { get; private set; }
        public int ServerPort { get; private set; }
        public LoginForm()
        {
            InitializeComponent();
        }

        private void chkShowPassword_CheckedChanged(object sender, EventArgs e)
        {
            if (chkShowPassword.Checked)
            {
                txtPassword.UseSystemPasswordChar = false;
            }
            else
            {
                txtPassword.UseSystemPasswordChar = true;
            }
        }


        private async void btnLogin_Click(object sender, EventArgs e)
        {
            //Ktra ServerIP có rỗng không
            if (string.IsNullOrWhiteSpace(txtServerIP.Text))
            {
                MessageBox.Show(
                    "Vui lòng nhập Server IP.",
                    "Thông báo",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );

                txtServerIP.Focus();
                return;
            }
            //Ktra định dạng IP
            if (!IPAddress.TryParse(txtServerIP.Text.Trim(), out IPAddress serverIP))
            {
                MessageBox.Show(
                    "Server IP không đúng định dạng.",
                    "Thông báo",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );

                txtServerIP.Focus();
                return;
            }
            //Ktra Port có phải số hay không, tránh TH nhập chữ
            if (!int.TryParse(txtPort.Text.Trim(), out int port))
            {
                MessageBox.Show(
                    "Port phải là một số.",
                    "Thông báo",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );

                txtPort.Focus();
                return;
            }
            //Ktra khoảng Port
            if (port < 1 || port > 65535)
            {
                MessageBox.Show(
                    "Port phải nằm trong khoảng từ 1 đến 65535.",
                    "Thông báo",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );

                txtPort.Focus();
                return;
            }
            //Ktra Username
            if (string.IsNullOrWhiteSpace(txtUsername.Text))
            {
                MessageBox.Show(
                    "Vui lòng nhập tên đăng nhập.",
                    "Thông báo",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );

                txtUsername.Focus();
                return;
            }
            //Ktra Password
            if (string.IsNullOrWhiteSpace(txtPassword.Text))
            {
                MessageBox.Show(
                    "Vui lòng nhập mật khẩu.",
                    "Thông báo",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );

                txtPassword.Focus();
                return;
            }

            btnLogin.Enabled = false;

            System.Net.Sockets.TcpClient tcp = null;
            bool loginSucceeded = false;

            try
            {
                string ip = txtServerIP.Text.Trim();
                string username = txtUsername.Text.Trim();
                string password = txtPassword.Text;

                // Gửi kết nối đến Server
                tcp = new System.Net.Sockets.TcpClient();

                Task connectTask = tcp.ConnectAsync(ip, port);

                if (await Task.WhenAny(connectTask, Task.Delay(5000)) != connectTask)
                {
                    throw new TimeoutException(
                        "Không thể kết nối đến Server trong 5 giây."
                    );
                }

                await connectTask;

                NetworkStream stream = tcp.GetStream();

                //Tạo yêu cầu để đăng nhập
                ChatMessage loginMessage = new ChatMessage
                {
                    Type = MessageType.Login,
                    Sender = username,
                    Password = password,
                    Timestamp = DateTime.Now
                };

                //Gửi yêu cầu đến Server
                await MessageSerializer.SendMessageAsync(
                    stream, loginMessage
                );

                //Chờ Server xác thực
                Task<ChatMessage> responseTask =
                    MessageSerializer.ReceiveMessageAsync(stream);

                if (await Task.WhenAny(responseTask, Task.Delay(10000)) != responseTask)
                {
                    throw new TimeoutException(
                        "Server không phản hồi yêu cầu đăng nhập."
                    );
                }

                ChatMessage response = await responseTask;

                if (response == null)
                {
                    throw new Exception("Server đã đóng kết nối.");
                }

                //Kiểm tra phản hồi từ Server
                if (response.Type != MessageType.LoginResult)
                {
                    throw new Exception(
                        "Server trả về phản hồi không hợp lệ."
                    );
                }

                if (!response.Success)
                {
                    MessageBox.Show(
                        response.Content ?? "Đăng nhập thất bại.",
                        "WeTALK",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning
                    );
                    return;
                }

                //Lưu phiên TCP đã xác thực
                AuthenticatedUser = username;
                TcpClient = tcp;
                Stream = stream;
                InitialMessage = response;
                ServerIP = ip;
                ServerPort = port;

                loginSucceeded = true;

                //Trả kết quả
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Không thể đăng nhập:\n" + ex.Message,
                    "Lỗi kết nối WeTALK",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
            finally
            {
                // Chỉ đóng kết nối nếu đăng nhập không thành công.
                // Khi thành công, ClientForm sẽ tiếp nhận phiên TCP.
                if (!loginSucceeded)
                {
                    tcp?.Close();
                }

                if (!this.IsDisposed)
                {
                    btnLogin.Enabled = true;
                }
            }


        }
        private void lnkRegister_LinkClicked(object sender,LinkLabelLinkClickedEventArgs e)
        {
            RegisterForm registerForm = new RegisterForm
            {
                ServerIP = txtServerIP.Text.Trim(),
                ServerPort = txtPort.Text.Trim()
            };

            registerForm.TopLevel = false;
            registerForm.FormBorderStyle = FormBorderStyle.None;

            registerForm.EnableEmbeddedMode();
            registerForm.Dock = DockStyle.Fill;

            pnlLogin.Controls.Add(registerForm);

            registerForm.BringToFront();
            registerForm.Show();
        }

    }
}
