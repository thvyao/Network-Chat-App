using System;
using System.Windows.Forms;

namespace ChatServer
{
    // Điểm khởi nhập chính (Entry Point) của tiến trình ứng dụng ChatServer.
    // Chạy độc lập trên một tiến trình riêng biệt theo yêu cầu của đề tài.
    internal static class Program
    {
        // Hàm Main khởi chạy ứng dụng máy chủ và mở form ServerForm.
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new ServerForm());
        }
    }
}
