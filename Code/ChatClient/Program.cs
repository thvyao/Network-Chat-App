using System;
using System.Windows.Forms;

namespace ChatClient
{
    internal static class Program
    {
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);

            bool showLogin = true;
            while (showLogin)
            {
                using (LoginForm loginForm = new LoginForm())
                {
                    if (loginForm.ShowDialog() != DialogResult.OK)
                    {
                        break;
                    }
                    try
                    {
                        using (ClientForm clientForm = new ClientForm(loginForm.AuthenticatedUser, loginForm.TcpClient, loginForm.Stream, loginForm.InitialMessage, loginForm.ServerIP, loginForm.ServerPort, loginForm.SessionPassword))
                        {
                            Application.Run(clientForm);
                            showLogin = clientForm.WantsRelogin;
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Đã xảy ra lỗi khi mở giao diện chat:\n" + ex.Message, "Lỗi Hệ Thống", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        showLogin = false;
                    }
                }
            }
        }
    }
}
