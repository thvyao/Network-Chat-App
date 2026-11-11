using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using ChatShared.Helpers;

namespace ChatClient
{
    // Hộp thoại Modal cho phép người dùng chọn đích đến khi chuyển tiếp (Forward) tin nhắn.
    // Cung cấp khung xem trước nội dung gốc và danh sách người nhận trực tuyến kèm avatar.
    public class ForwardForm : Form
    {
        private DisplayMessage messageToForward;
        private ListBox lstRecipients;
        private Button btnOk;
        private Button btnCancel;

        public string SelectedRecipient { get; private set; }

        // Khởi tạo form chuyển tiếp với tin nhắn cần chuyển và danh sách người nhận hiện có.
        public ForwardForm(DisplayMessage msg, List<string> availableRecipients)
        {
            this.messageToForward = msg;
            InitializeUI(availableRecipients);
        }

        // Khởi tạo và bố cục các thành phần giao diện của hộp thoại chuyển tiếp.
        private void InitializeUI(List<string> availableRecipients)
        {
            this.Text = "Chuyển tiếp tin nhắn";
            this.Size = new Size(360, 440);
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Font = new Font("Segoe UI", 9f);
            this.BackColor = Color.White;

            // 1. Khung xem trước (Preview Panel) hiển thị nội dung tin nhắn gốc
            Panel pnlPreview = new Panel
            {
                Dock = DockStyle.Top,
                Height = 70,
                Padding = new Padding(12, 10, 12, 8),
                BackColor = Color.FromArgb(245, 246, 247)
            };

            Label lblPreviewTitle = new Label
            {
                Text = "Nội dung chuyển tiếp:",
                Font = new Font("Segoe UI", 8.5f, FontStyle.Bold),
                ForeColor = Color.FromArgb(100, 100, 100),
                Dock = DockStyle.Top,
                Height = 18
            };

            string preview = messageToForward.Content;
            if (preview.Length > 80) preview = preview.Substring(0, 77) + "...";
            Label lblPreview = new Label
            {
                Text = $"\"{preview}\"",
                Font = new Font("Segoe UI", 9f, FontStyle.Italic),
                ForeColor = Color.FromArgb(50, 50, 50),
                Dock = DockStyle.Fill
            };
            pnlPreview.Controls.Add(lblPreview);
            pnlPreview.Controls.Add(lblPreviewTitle);

            // 2. Nhãn hướng dẫn chọn người nhận
            Label lblSelect = new Label
            {
                Text = "Chọn người nhận:",
                Font = new Font("Segoe UI", 9f, FontStyle.Bold),
                Dock = DockStyle.Top,
                Height = 30,
                Padding = new Padding(12, 8, 0, 0)
            };

            // 3. Danh sách người nhận tự vẽ (OwnerDraw) để hiển thị kèm Avatar
            lstRecipients = new ListBox
            {
                Dock = DockStyle.Fill,
                DrawMode = DrawMode.OwnerDrawFixed,
                ItemHeight = 44,
                BorderStyle = BorderStyle.None,
                BackColor = Color.White
            };

            foreach (var r in availableRecipients)
            {
                lstRecipients.Items.Add(r);
            }
            if (lstRecipients.Items.Count > 0) lstRecipients.SelectedIndex = 0;

            lstRecipients.DrawItem += (s, e) =>
            {
                if (e.Index < 0 || e.Index >= lstRecipients.Items.Count) return;
                bool isSelected = (e.State & DrawItemState.Selected) == DrawItemState.Selected;
                string item = (string)lstRecipients.Items[e.Index];

                using (var bgBrush = new SolidBrush(isSelected ? Color.FromArgb(231, 243, 255) : Color.White))
                {
                    e.Graphics.FillRectangle(bgBrush, e.Bounds);
                }

                // Tạo avatar tương ứng cho kênh Tất cả hoặc người dùng
                Image avatar;
                if (string.Equals(item, "Tất cả (Phòng chung)",StringComparison.Ordinal))
                {
                    avatar = AvatarHelper.GenerateAvatar("👥", 32);
                }
                else
                {
                    avatar = AvatarHelper.GenerateAvatar(item, 32);
                }
                e.Graphics.DrawImage(avatar, e.Bounds.X + 12, e.Bounds.Y + 6, 32, 32);

                // Vẽ tên người nhận bên cạnh avatar
                using (var f = new Font("Segoe UI", 9.5f, isSelected ? FontStyle.Bold : FontStyle.Regular))
                using (var brush = new SolidBrush(Color.FromArgb(20, 20, 20)))
                {
                    e.Graphics.DrawString(item, f, brush, e.Bounds.X + 52, e.Bounds.Y + 12);
                }
            };

            // 4. Khung điều khiển chân trang (Chuyển tiếp và Hủy)
            Panel pnlBottom = new Panel
            {
                Dock = DockStyle.Bottom,
                Height = 55,
                Padding = new Padding(12),
                BackColor = Color.FromArgb(245, 246, 247)
            };

            btnOk = new Button
            {
                Text = "Chuyển tiếp",
                Size = new Size(100, 34),
                Location = new Point(pnlBottom.Width - 115, 10),
                Anchor = AnchorStyles.Right | AnchorStyles.Top,
                BackColor = Color.FromArgb(0, 132, 255),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9f, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnOk.FlatAppearance.BorderSize = 0;
            btnOk.Click += (s, e) =>
            {
                if (lstRecipients.SelectedItem != null)
                {
                    SelectedRecipient = (string)lstRecipients.SelectedItem;
                    this.DialogResult = DialogResult.OK;
                    this.Close();
                }
            };

            btnCancel = new Button
            {
                Text = "Hủy",
                DialogResult = DialogResult.Cancel,
                Size = new Size(80, 34),
                Location = new Point(pnlBottom.Width - 205, 10),
                Anchor = AnchorStyles.Right | AnchorStyles.Top,
                BackColor = Color.FromArgb(228, 230, 235),
                ForeColor = Color.FromArgb(20, 20, 20),
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9f),
                Cursor = Cursors.Hand
            };
            btnCancel.FlatAppearance.BorderSize = 0;

            pnlBottom.Controls.Add(btnCancel);
            pnlBottom.Controls.Add(btnOk);

            this.Controls.Add(lstRecipients);
            this.Controls.Add(lblSelect);
            this.Controls.Add(pnlPreview);
            this.Controls.Add(pnlBottom);

            this.AcceptButton = btnOk;
            this.CancelButton = btnCancel;
        }
    }
}
