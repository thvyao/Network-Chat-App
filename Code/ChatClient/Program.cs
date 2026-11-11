
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

            using (LoginForm loginForm = new LoginForm())
            {
                if (loginForm.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        ClientForm clientForm = new ClientForm(
                            loginForm.AuthenticatedUser,
                            loginForm.TcpClient,
                            loginForm.Stream,
                            loginForm.InitialMessage,
                            loginForm.ServerIP,
                            loginForm.ServerPort
                        );

                        Application.Run(clientForm);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(
                            "Đã xảy ra lỗi khi mở giao diện chat:\n" + ex.Message,
                            "Lỗi Hệ Thống",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Error
                        );
                    }
                }
            }
        }
    }
}