using System;
using System.Drawing;
using System.Windows.Forms;
using ChatShared.Helpers;

namespace ChatClient
{
    // Cửa sổ Popup không viền cho phép người dùng lựa chọn biểu tượng cảm xúc (Emoji).
    // Hỗ trợ chuyển tab nhanh chóng giữa các nhóm chủ đề và tự động đóng khi mất tiêu điểm (Deactivate) hoặc ấn phím Escape.
    public class EmojiPickerForm : Form
    { 
        public event Action<string> OnEmojiSelected;
        public event Action OnEnterPressed;
        private FlowLayoutPanel flowEmojis;
        private Panel pnlCategories;
        private Button activeButton = null;

        // Khởi tạo popup bộ chọn biểu tượng cảm xúc.
        public EmojiPickerForm()
        {
            InitializeUI();
        }

        // Bắt các phím điều hướng hệ thống (Enter để gửi tin, Escape để đóng)
        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == Keys.Enter)
            {
                this.Close();
                OnEnterPressed?.Invoke();
                return true; 
            }
            if (keyData == Keys.Escape)
            {
                this.Close();
                return true;
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }

        // Cấu hình các điều khiển giao diện (UI) cho Popup bộ chọn Emoji.
        private void InitializeUI()
        {
            this.FormBorderStyle = FormBorderStyle.None;
            this.ShowInTaskbar = false;
            this.StartPosition = FormStartPosition.Manual;
            this.Size = new Size(350, 300);
            this.BackColor = Color.White;
            this.KeyPreview = true;

            this.Deactivate += (s, e) => this.Close();

            Panel pnlHeader = new Panel
            {
                Dock = DockStyle.Top,
                Height = 36,
                BackColor = Color.FromArgb(245, 246, 247),
                Padding = new Padding(10, 0, 8, 0)
            };

            Label lblTitle = new Label
            {
                Text = "Biểu tượng cảm xúc",
                Font = new Font("Segoe UI", 9f, FontStyle.Bold),
                ForeColor = Color.FromArgb(80, 80, 80),
                AutoSize = false,
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleLeft
            };

            Button btnClose = new Button
            {
                Text = "✕",
                Size = new Size(26, 26),
                Dock = DockStyle.Right,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9f),
                ForeColor = Color.FromArgb(120, 120, 120),
                Cursor = Cursors.Hand
            };
            btnClose.FlatAppearance.BorderSize = 0;
            btnClose.Click += (s, e) => this.Close();

            pnlHeader.Controls.Add(lblTitle);
            pnlHeader.Controls.Add(btnClose);

            pnlCategories = new Panel
            {
                Dock = DockStyle.Top,
                Height = 36,
                BackColor = Color.FromArgb(240, 242, 245),
                Padding = new Padding(4, 3, 4, 3)
            };

            flowEmojis = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                AutoScroll = true,
                BackColor = Color.White,
                Padding = new Padding(6)
            };

            int left = 4;
            foreach (var kvp in EmojiHelper.EmojiCategories)
            {
                string categoryName = kvp.Key;
                string icon = kvp.Value.Length > 0 ? kvp.Value[0] : "";

                Button btnCat = new Button
                {
                    Text = $"{icon} {categoryName}",
                    Font = new Font("Segoe UI Emoji", 8f),
                    Size = new Size(82, 30),
                    Location = new Point(left, 3),
                    FlatStyle = FlatStyle.Flat,
                    BackColor = Color.Transparent,
                    ForeColor = Color.FromArgb(60, 60, 60),
                    Cursor = Cursors.Hand,
                    Tag = categoryName
                };
                btnCat.FlatAppearance.BorderSize = 0;
                btnCat.Click += (s, e) => SwitchCategory((Button)s, categoryName);

                pnlCategories.Controls.Add(btnCat);
                left += 86;

                if (activeButton == null)
                {
                    activeButton = btnCat;
                }
            }

            this.Controls.Add(flowEmojis);
            this.Controls.Add(pnlCategories);
            this.Controls.Add(pnlHeader);

            if (activeButton != null)
            {
                SwitchCategory(activeButton, (string)activeButton.Tag);
            }
        }

        // Xử lý chuyển đổi danh mục: làm nổi bật tab được chọn và nạp các emoji tương ứng vào FlowLayoutPanel.
        private void SwitchCategory(Button btn, string categoryName)
        {
            if (activeButton != null)
            {
                activeButton.BackColor = Color.Transparent;
                activeButton.Font = new Font("Segoe UI Emoji", 8f, FontStyle.Regular);
            }

            activeButton = btn;
            activeButton.BackColor = Color.White;
            activeButton.Font = new Font("Segoe UI Emoji", 8f, FontStyle.Bold);

            flowEmojis.SuspendLayout();
            flowEmojis.Controls.Clear();

            // Nạp toàn bộ emoji trong danh mục được chọn
            if (EmojiHelper.EmojiCategories.TryGetValue(categoryName, out string[] emojis))
            {
                foreach (var emoji in emojis)
                {
                    Label lblEmoji = new Label
                    {
                        Text = emoji,
                        Font = new Font("Segoe UI Emoji", 14f),
                        Size = new Size(36, 36),
                        TextAlign = ContentAlignment.MiddleCenter,
                        Cursor = Cursors.Hand,
                        Margin = new Padding(2)
                    };

                    lblEmoji.MouseEnter += (s, e) => lblEmoji.BackColor = Color.FromArgb(230, 235, 245);
                    lblEmoji.MouseLeave += (s, e) => lblEmoji.BackColor = Color.Transparent;
                    
                    lblEmoji.Click += (s, e) =>
                    {
                        OnEmojiSelected?.Invoke(emoji);
                    };

                    flowEmojis.Controls.Add(lblEmoji);
                }
            }

            flowEmojis.ResumeLayout(true);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            using (var p = new Pen(Color.FromArgb(190, 195, 205), 1))
            {
                e.Graphics.DrawRectangle(p, 0, 0, Width - 1, Height - 1);
            }
        }
    }
}
