using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Windows.Forms;

namespace ChatClient
{
    public class MessageBubbleControl : UserControl
    {
        public DisplayMessage Message { get; private set; } 
        public Image AvatarImage { get; private set; }
        public bool IsOwnMessage => Message != null && Message.IsOwn; 
        public event Action<DisplayMessage> OnReplyClicked;
        public event Action<DisplayMessage> OnForwardClicked;

        private Rectangle avatarRect;
        private Rectangle bubbleRect;
        private Rectangle senderNameRect;
        private Rectangle forwardRect;
        private Rectangle replyBoxRect;
        private Rectangle replyBarRect;
        private Rectangle replySenderRect;
        private Rectangle replyTextRect;
        private Rectangle contentRect;
        private Rectangle timestampRect;
        private Rectangle systemPillRect;

        // Định nghĩa các font chữ sử dụng kết xuất
        private static readonly Font SenderFont = new Font("Segoe UI", 9f, FontStyle.Bold);
        private static readonly Font ContentFont = new Font("Segoe UI Emoji", 9.5f, FontStyle.Regular);
        private static readonly Font ForwardFont = new Font("Segoe UI", 8f, FontStyle.Italic);
        private static readonly Font ReplySenderFont = new Font("Segoe UI", 8f, FontStyle.Bold);
        private static readonly Font ReplyContentFont = new Font("Segoe UI", 8f, FontStyle.Regular);
        private static readonly Font TimestampFont = new Font("Segoe UI", 7.5f, FontStyle.Regular);
        private static readonly Font SystemFont = new Font("Segoe UI", 8.5f, FontStyle.Italic);

        // Bảng màu giao diện bong bóng chat
        private static readonly Color OwnBubbleColor = Color.FromArgb(0, 132, 255);       // Xanh dương Messenger
        private static readonly Color OtherBubbleColor = Color.FromArgb(228, 230, 235);    // Xám nhạt
        private static readonly Color OwnTextColor = Color.White;
        private static readonly Color OtherTextColor = Color.FromArgb(5, 5, 5);
        private static readonly Color TimestampColor = Color.FromArgb(142, 142, 147);
        private static readonly Color OtherSenderNameColor = Color.FromArgb(100, 100, 100);

        private ContextMenuStrip contextMenu;
        
        public MessageBubbleControl(DisplayMessage message, Image avatar, int initialWidth)
        {
            this.Message = message;
            this.AvatarImage = avatar;

            SetStyle(ControlStyles.AllPaintingInWmPaint |
                     ControlStyles.UserPaint |
                     ControlStyles.OptimizedDoubleBuffer |
                     ControlStyles.ResizeRedraw, true);
            UpdateStyles();
            InitializeContextMenu();
            UpdateWidth(initialWidth);
        }

        private void InitializeContextMenu()
        {
            if (Message == null || Message.IsSystem) return;

            contextMenu = new ContextMenuStrip();
            contextMenu.Font = new Font("Segoe UI", 9f);

            var itemReply = contextMenu.Items.Add("💬  Trả lời");
            itemReply.Click += (s, e) => OnReplyClicked?.Invoke(Message);

            var itemForward = contextMenu.Items.Add("➡️  Chuyển tiếp");
            itemForward.Click += (s, e) => OnForwardClicked?.Invoke(Message);

            contextMenu.Items.Add(new ToolStripSeparator());

            var itemCopy = contextMenu.Items.Add("📋  Sao chép tin nhắn");
            itemCopy.Click += (s, e) =>
            {
                if (!string.IsNullOrEmpty(Message?.Content))
                {
                    try { Clipboard.SetText(Message.Content); } catch { }
                }
            };

            this.ContextMenuStrip = contextMenu;
        }

        public void UpdateWidth(int availableWidth)
        {
            if (availableWidth < 180) availableWidth = 180;
            this.Width = availableWidth;
            CalculateLayout();
        }

        private Size MeasureString(string text, Font font, int maxWidth)
        {
            if (string.IsNullOrEmpty(text)) return Size.Empty;
            return TextRenderer.MeasureText(text, font, new Size(maxWidth, int.MaxValue),
                TextFormatFlags.WordBreak | TextFormatFlags.TextBoxControl);
        }

        private void CalculateLayout()
        {
            if (Message == null) return;

            int w = this.Width;

            // 1. Xử lý tin nhắn hệ thống (dạng viên thuốc hình bầu dục pill căn giữa)
            if (Message.IsSystem)
            {
                string sysText = $"[ {Message.Timestamp:HH:mm} ]  {Message.Content}";
                Size sysSize = MeasureString(sysText, SystemFont, Math.Min(w - 60, 600));
                int pillW = sysSize.Width + 24;
                int pillH = sysSize.Height + 10;
                int pillX = (w - pillW) / 2;
                int pillY = 6;
                systemPillRect = new Rectangle(pillX, pillY, pillW, pillH);
                this.Height = pillH + 12;
                return;
            }

            // 2. Thiết lập thông số hình học cho tin nhắn người dùng
            int avatarSize = 36;
            int margin = 10;
            int maxBubbleWidth = Math.Max(160, Math.Min(600, (int)(w * 0.65f)));
            int innerMaxContentWidth = maxBubbleWidth - 24;

            bool hasForward = !string.IsNullOrEmpty(Message.ForwardFrom);
            bool hasReply = !string.IsNullOrEmpty(Message.ReplyToContent);

            // Measure Forward
            Size fwdSize = Size.Empty;
            string fwdText = hasForward ? $"↪ Chuyển tiếp từ: {Message.ForwardFrom}" : "";
            if (hasForward)
            {
                fwdSize = MeasureString(fwdText, ForwardFont, innerMaxContentWidth);
            }

            // Measure Reply
            Size replySenderSize = Size.Empty;
            Size replyContentSize = Size.Empty;
            string repSender = hasReply ? (Message.ReplyToSender ?? "Người dùng") : "";
            string repContent = hasReply ? Message.ReplyToContent : "";
            if (hasReply)
            {
                if (repContent.Length > 60) repContent = repContent.Substring(0, 57) + "...";
                replySenderSize = MeasureString(repSender, ReplySenderFont, innerMaxContentWidth - 14);
                replyContentSize = MeasureString(repContent, ReplyContentFont, innerMaxContentWidth - 14);
            }

            // Measure Content
            Size contentSize = MeasureString(Message.Content, ContentFont, innerMaxContentWidth);

            // Calculate bubble width
            int contentWidth = contentSize.Width;
            if (hasForward && fwdSize.Width > contentWidth) contentWidth = fwdSize.Width;
            if (hasReply)
            {
                int repBoxTextW = Math.Max(replySenderSize.Width, replyContentSize.Width) + 14;
                if (repBoxTextW > contentWidth) contentWidth = repBoxTextW;
            }

            int bubbleWidth = Math.Max(70, Math.Min(contentWidth + 24, maxBubbleWidth));
            int actualInnerWidth = bubbleWidth - 24;

            // Recalculate heights with actual inner width
            if (hasForward)
            {
                fwdSize = MeasureString(fwdText, ForwardFont, actualInnerWidth);
            }
            if (hasReply)
            {
                replySenderSize = MeasureString(repSender, ReplySenderFont, actualInnerWidth - 14);
                replyContentSize = MeasureString(repContent, ReplyContentFont, actualInnerWidth - 14);
            }
            contentSize = MeasureString(Message.Content, ContentFont, actualInnerWidth);

            // Vertical layout inside bubble
            int innerY = 8;

            if (hasForward)
            {
                forwardRect = new Rectangle(12, innerY, actualInnerWidth, fwdSize.Height);
                innerY += fwdSize.Height + 5;
            }
            else
            {
                forwardRect = Rectangle.Empty;
            }

            if (hasReply)
            {
                int repBoxH = replySenderSize.Height + replyContentSize.Height + 8;
                replyBoxRect = new Rectangle(12, innerY, actualInnerWidth, repBoxH);
                replyBarRect = new Rectangle(12, innerY, 3, repBoxH);
                replySenderRect = new Rectangle(19, innerY + 3, actualInnerWidth - 11, replySenderSize.Height);
                replyTextRect = new Rectangle(19, innerY + 3 + replySenderSize.Height, actualInnerWidth - 11, replyContentSize.Height);
                innerY += repBoxH + 6;
            }
            else
            {
                replyBoxRect = Rectangle.Empty;
            }

            contentRect = new Rectangle(12, innerY, actualInnerWidth, contentSize.Height);
            innerY += contentSize.Height + 8;

            int bubbleHeight = innerY;

            // Measure timestamp
            string timeStr = Message.Timestamp.ToString("HH:mm");
            Size timeSize = MeasureString(timeStr, TimestampFont, 100);

            // Position components based on Own vs Other
            if (IsOwnMessage)
            {
                avatarRect = new Rectangle(w - avatarSize - margin, 6, avatarSize, avatarSize);
                int bubbleX = w - avatarSize - margin - 8 - bubbleWidth;
                int bubbleY = 6;
                bubbleRect = new Rectangle(bubbleX, bubbleY, bubbleWidth, bubbleHeight);

                senderNameRect = Rectangle.Empty;
                OffsetInnerRects(bubbleX, bubbleY);
                timestampRect = new Rectangle(bubbleRect.Right - timeSize.Width, bubbleRect.Bottom + 2, timeSize.Width, timeSize.Height);

                this.Height = Math.Max(bubbleRect.Bottom + timeSize.Height + 6, avatarSize + 14);
            }
            else
            {
                avatarRect = new Rectangle(margin, 6, avatarSize, avatarSize);

                Size senderSize = MeasureString(Message.Sender ?? "Người dùng", SenderFont, maxBubbleWidth);
                int bubbleX = margin + avatarSize + 8;
                int senderY = 4;
                senderNameRect = new Rectangle(bubbleX + 2, senderY, senderSize.Width, senderSize.Height);

                int bubbleY = senderY + senderSize.Height + 3;
                bubbleRect = new Rectangle(bubbleX, bubbleY, bubbleWidth, bubbleHeight);

                OffsetInnerRects(bubbleX, bubbleY);
                timestampRect = new Rectangle(bubbleX + 4, bubbleRect.Bottom + 2, timeSize.Width, timeSize.Height);

                this.Height = Math.Max(bubbleRect.Bottom + timeSize.Height + 6, avatarSize + 14);
            }
        }

        private void OffsetInnerRects(int bx, int by)
        {
            if (!forwardRect.IsEmpty)
                forwardRect = new Rectangle(bx + forwardRect.X, by + forwardRect.Y, forwardRect.Width, forwardRect.Height);

            if (!replyBoxRect.IsEmpty)
            {
                replyBoxRect = new Rectangle(bx + replyBoxRect.X, by + replyBoxRect.Y, replyBoxRect.Width, replyBoxRect.Height);
                replyBarRect = new Rectangle(bx + replyBarRect.X, by + replyBarRect.Y, replyBarRect.Width, replyBarRect.Height);
                replySenderRect = new Rectangle(bx + replySenderRect.X, by + replySenderRect.Y, replySenderRect.Width, replySenderRect.Height);
                replyTextRect = new Rectangle(bx + replyTextRect.X, by + replyTextRect.Y, replyTextRect.Width, replyTextRect.Height);
            }

            contentRect = new Rectangle(bx + contentRect.X, by + contentRect.Y, contentRect.Width, contentRect.Height);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;

            if (Message == null) return;

            if (Message.IsSystem)
            {
                DrawSystemMessage(g);
                return;
            }

            // Draw Avatar
            if (AvatarImage != null)
            {
                g.DrawImage(AvatarImage, avatarRect);
            }
            else
            {
                using (var b = new SolidBrush(Color.Gray))
                {
                    g.FillEllipse(b, avatarRect);
                }
            }

            // Draw Sender Name (only for others)
            if (!IsOwnMessage && !senderNameRect.IsEmpty)
            {
                TextRenderer.DrawText(g, Message.Sender ?? "", SenderFont, senderNameRect, OtherSenderNameColor,
                    TextFormatFlags.SingleLine | TextFormatFlags.EndEllipsis);
            }

            // Draw Bubble
            using (GraphicsPath path = CreateRoundedRectangle(bubbleRect, 14))
            using (SolidBrush bubbleBrush = new SolidBrush(IsOwnMessage ? OwnBubbleColor : OtherBubbleColor))
            {
                g.FillPath(bubbleBrush, path);
            }

            // Draw Forward Header
            if (!forwardRect.IsEmpty)
            {
                Color fwdColor = IsOwnMessage ? Color.FromArgb(215, 235, 255) : Color.FromArgb(100, 100, 100);
                string fwdText = $"↪ Chuyển tiếp từ: {Message.ForwardFrom}";
                TextRenderer.DrawText(g, fwdText, ForwardFont, forwardRect, fwdColor,
                    TextFormatFlags.WordBreak | TextFormatFlags.TextBoxControl);
            }

            // Draw Reply Quote Box
            if (!replyBoxRect.IsEmpty)
            {
                Color repBg = IsOwnMessage ? Color.FromArgb(45, 255, 255, 255) : Color.FromArgb(215, 218, 222);
                Color repBarColor = IsOwnMessage ? Color.White : OwnBubbleColor;
                Color repSenderColor = IsOwnMessage ? Color.White : Color.FromArgb(30, 30, 30);
                Color repTextColor = IsOwnMessage ? Color.FromArgb(230, 240, 255) : Color.FromArgb(80, 80, 80);

                using (GraphicsPath repPath = CreateRoundedRectangle(replyBoxRect, 6))
                using (SolidBrush repBrush = new SolidBrush(repBg))
                {
                    g.FillPath(repBrush, repPath);
                }

                using (SolidBrush barBrush = new SolidBrush(repBarColor))
                {
                    g.FillRectangle(barBrush, replyBarRect);
                }

                TextRenderer.DrawText(g, Message.ReplyToSender ?? "Người dùng", ReplySenderFont, replySenderRect, repSenderColor,
                    TextFormatFlags.SingleLine | TextFormatFlags.EndEllipsis);

                string repContent = Message.ReplyToContent ?? "";
                if (repContent.Length > 60) repContent = repContent.Substring(0, 57) + "...";
                TextRenderer.DrawText(g, repContent, ReplyContentFont, replyTextRect, repTextColor,
                    TextFormatFlags.SingleLine | TextFormatFlags.EndEllipsis);
            }

            // Draw Content
            Color textColor = IsOwnMessage ? OwnTextColor : OtherTextColor;
            TextRenderer.DrawText(g, Message.Content, ContentFont, contentRect, textColor,
                TextFormatFlags.WordBreak | TextFormatFlags.TextBoxControl);

            // Draw Timestamp
            string timeStr = Message.Timestamp.ToString("HH:mm");
            TextRenderer.DrawText(g, timeStr, TimestampFont, timestampRect, TimestampColor,
                TextFormatFlags.SingleLine);
        }

        private void DrawSystemMessage(Graphics g)
        {
            using (GraphicsPath path = CreateRoundedRectangle(systemPillRect, 10))
            using (SolidBrush bgBrush = new SolidBrush(Color.FromArgb(240, 242, 245)))
            {
                g.FillPath(bgBrush, path);
            }

            string sysText = $"[ {Message.Timestamp:HH:mm} ]  {Message.Content}";
            TextRenderer.DrawText(g, sysText, SystemFont, systemPillRect, Color.FromArgb(100, 100, 100),
                TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter | TextFormatFlags.WordBreak);
        }

        private GraphicsPath CreateRoundedRectangle(Rectangle rect, int radius)
        {
            GraphicsPath path = new GraphicsPath();
            if (rect.Width <= 0 || rect.Height <= 0) return path;

            int d = radius * 2;
            if (d > rect.Width) d = rect.Width;
            if (d > rect.Height) d = rect.Height;

            path.AddArc(rect.X, rect.Y, d, d, 180, 90);
            path.AddArc(rect.Right - d, rect.Y, d, d, 270, 90);
            path.AddArc(rect.Right - d, rect.Bottom - d, d, d, 0, 90);
            path.AddArc(rect.X, rect.Bottom - d, d, d, 90, 90);
            path.CloseFigure();
            return path;
        }
    }
}
