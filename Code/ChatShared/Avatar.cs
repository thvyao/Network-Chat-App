using System;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace ChatShared.Helpers
{
        private static readonly Color[] AvatarColors = new Color[]
        {
            Color.FromArgb(66, 133, 244),   // Xanh dương Google Blue
            Color.FromArgb(234, 67, 53),    // Đỏ Red
            Color.FromArgb(251, 188, 4),    // Vàng Yellow
            Color.FromArgb(52, 168, 83),    // Xanh lá Green
            Color.FromArgb(171, 71, 188),   // Tím Purple
            Color.FromArgb(255, 112, 67),   // Cam Orange
            Color.FromArgb(0, 172, 193),    // Xanh lơ Cyan
            Color.FromArgb(124, 179, 66),   // Xanh non Light Green
            Color.FromArgb(255, 167, 38),   // Hổ phách Amber
            Color.FromArgb(239, 83, 80),    // Đỏ nhạt Coral
        };

        public static Image GenerateAvatar(string username, int size = 40)
        {
            if (string.IsNullOrEmpty(username)) username = "?";
            int colorIndex = Math.Abs(username.GetHashCode()) % AvatarColors.Length;
            Color bgColor = AvatarColors[colorIndex];
            string initial = username.Substring(0, 1).ToUpper();

            Bitmap bmp = new Bitmap(size, size);
            using (Graphics g = Graphics.FromImage(bmp))
            {
                g.SmoothingMode = SmoothingMode.AntiAlias;
                g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;

                using (SolidBrush brush = new SolidBrush(bgColor))
                {
                    g.FillEllipse(brush, 0, 0, size - 1, size - 1);
                }

                using (Font font = new Font("Segoe UI", size * 0.45f, FontStyle.Bold))
                using (SolidBrush textBrush = new SolidBrush(Color.White))
                {
                    StringFormat sf = new StringFormat
                    {
                        Alignment = StringAlignment.Center,
                        LineAlignment = StringAlignment.Center
                    };
                    g.DrawString(initial, font, textBrush, new RectangleF(0, 0, size, size), sf);
                }
            }
            return bmp;
        }
    }
}
