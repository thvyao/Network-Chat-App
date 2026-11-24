using System;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net;
using System.Net.Sockets;
using ChatShared.Helpers;
using ChatShared.Protocol;
using System.Drawing;

namespace ChatClient
{
    public partial class RegisterForm : Form
    {
        // Thông tin Server được truyền từ LoginForm
        public string ServerIP { get; set; } = "127.0.0.1";
        public string ServerPort { get; set; } = "2327";
        public RegisterForm()
        {
            InitializeComponent();
            chkShowPassword.Font = new Font("Segoe UI", 9F);
            lblRegisterSub.Font = new Font("Segoe UI", 9F);

            AlignLayout();
            pnlRegister.SizeChanged += (s, e) => AlignLayout();

            this.ActiveControl = txtUsername;
        }

        private void AlignLayout()
        {
            const int w = 388;
            int panelW = pnlRegister.ClientSize.Width;
            int left = (panelW - w) / 2;

            lblRegisterTitle.Left = (panelW - lblRegisterTitle.Width) / 2;
            lblRegisterSub.Left = (panelW - lblRegisterSub.Width) / 2;

            foreach (Control c in new Control[] { lblUsername, lblPassword, lblConfirmPassword })
                c.Left = left;

            foreach (Control c in new Control[] { txtUsername, txtPassword, txtConfirmPassword, btnRegister })
                c.SetBounds(left, c.Top, w, c.Height);

            chkShowPassword.Left = left - 2;
            lnkLogin.Left = (panelW - lnkLogin.Width) / 2;

            lblSlogan.AutoSize = true;
            lblSlogan.TextAlign = ContentAlignment.TopLeft;
            lblSlogan.Location = new Point(lblBrandName.Left + 4, lblBrandName.Bottom - 6);
        }

        public void EnableEmbeddedMode()
        {
            pnlBrand.Visible = false;
            pnlRegister.Dock = DockStyle.Fill;
        }
        private void chkShowPassword_CheckedChanged(object sender, EventArgs e)
        {
            if (chkShowPassword.Checked)
            {
                txtPassword.UseSystemPasswordChar = false;
                txtConfirmPassword.UseSystemPasswordChar = false;
            }
            else
            {
                txtPassword.UseSystemPasswordChar = true;
                txtConfirmPassword.UseSystemPasswordChar = true;
            }
        }



        private async void btnRegister_Click(object sender, EventArgs e)
        {
            //Ktra tên đăng nhập
            string username = txtUsername.Text.Trim();

            if (username.Length < 2 || username.Length > 25)
            {
                MessageBox.Show(
                    "Tên đăng nhập phải có từ 2 đến 25 ký tự.",
                    "WeTALK",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );
                txtUsername.Focus();
                return;
            }

            //Ktra mật khẩu
            string password = txtPassword.Text;

            if (password.Length < 4)
            {
                MessageBox.Show(
                    "Mật khẩu phải có ít nhất 4 ký tự.",
                    "WeTALK",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );
                txtPassword.Focus();
                return;
            }

            //Ktra xác nhận mật khẩu
            if (password != txtConfirmPassword.Text)
            {
                MessageBox.Show(
                    "Mật khẩu xác nhận không khớp.",
                    "WeTALK",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );
                txtConfirmPassword.Focus();
                return;
            }

            //Ktra địa chỉ IP
            string ip = ServerIP.Trim();

            if (!IPAddress.TryParse(ip, out _))
            {
                MessageBox.Show(
                    "Địa chỉ máy chủ không hợp lệ. Hãy kiểm tra tại trang đăng nhập.",
                    "WeTALK",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );
                return;
            }

            //Ktra Port
            if (!int.TryParse(ServerPort.Trim(), out int port)
                || port < 1 || port > 65535)
            {
                MessageBox.Show(
                    "Port không hợp lệ. Hãy kiểm tra tại trang đăng nhập.",
                    "WeTALK",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );
                return;
            }

            //Khóa nút để tránh gửi nhiều yc
            btnRegister.Enabled = false;

            try
            {
                using (TcpClient tcp = new TcpClient())
                {
                    // Kết nối tới SV
                    Task connectTask = tcp.ConnectAsync(ip, port);

                    if (await Task.WhenAny(
                            connectTask, Task.Delay(5000)) != connectTask)
                    {
                        tcp.Close();
                        throw new TimeoutException(
                            "Không thể kết nối đến Server trong 5 giây."
                        );
                    }

                    await connectTask;

                    using (NetworkStream stream = tcp.GetStream())
                    {
                        //Tạo thông điệp dk
                        ChatMessage registerMessage = new ChatMessage
                        {
                            Type = MessageType.Register,
                            Sender = username,
                            Password = password,
                            Timestamp = DateTime.Now
                        };

                        //Gửi yc đến SV
                        await MessageSerializer.SendMessageAsync(
                            stream, registerMessage
                        );

                        //Chờ phản hồi từ SV
                        Task<ChatMessage> responseTask =
                            MessageSerializer.ReceiveMessageAsync(stream);

                        if (await Task.WhenAny(
                                responseTask, Task.Delay(10000)) != responseTask)
                        {
                            throw new TimeoutException(
                                "Server không phản hồi yêu cầu đăng ký."
                            );
                        }

                        ChatMessage response = await responseTask;

                        if (response == null)
                        {
                            throw new Exception(
                                "Server đã đóng kết nối."
                            );
                        }

                        //Ktra loại phản hồi
                        if (response.Type != MessageType.RegisterResult)
                        {
                            throw new Exception(
                                "Phản hồi đăng ký không hợp lệ."
                            );
                        }

                        //SV xác nhận đăng ký thành công
                        if (response.Success)
                        {
                            MessageBox.Show(
                                response.Content
                                    ?? "Đăng ký thành công! Hãy đăng nhập.",
                                "WeTALK",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Information
                            );

                            //Quay về LoginForm
                            this.Close();
                            return;
                        }

                        //SV từ chối đăng ký
                        MessageBox.Show(
                            response.Content ?? "Đăng ký thất bại.",
                            "WeTALK",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Warning
                        );
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Không thể đăng ký:\n" + ex.Message,
                    "Lỗi kết nối WeTALK",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
            finally
            {
                if (!this.IsDisposed && !this.Disposing)
                {
                    btnRegister.Enabled = true;
                }
            }
        }


        private void lnkLogin_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            this.Close();
        }
    }
}

