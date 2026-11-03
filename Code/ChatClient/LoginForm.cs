using System;
using System.Windows.Forms;
using System.Net;

namespace ChatClient
{
    public partial class LoginForm : Form
    {
        public LoginForm()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void lblSlogan_Click(object sender, EventArgs e)
        {

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


        private void btnLogin_Click(object sender, EventArgs e)
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
                MessageBox.Show(
                    "Thông tin nhập vào hợp lệ.",
                    "WeTALK",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information
                );

        }

        private void lnkRegister_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            using (RegisterForm registerForm = new RegisterForm()) 
            {
                registerForm.ShowDialog(this);
             }
        }
    }
}
