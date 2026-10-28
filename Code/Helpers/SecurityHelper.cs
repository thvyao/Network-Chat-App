using System;
using System.Security.Cryptography;
using System.Text;

namespace ChatShared.Helpers
{
    // Lớp tiện ích bảo mật hỗ trợ sinh Salt ngẫu nhiên và băm mật khẩu bằng thuật toán SHA-256.
    // Giúp lưu trữ mật khẩu an toàn trên máy chủ và chống tấn công Rainbow Table.
    public static class SecurityHelper
    {
        public static string GenerateSalt(int size = 16)
        {
            byte[] saltBytes = new byte[size];
            using (var rng = new RNGCryptoServiceProvider())
            {
                rng.GetBytes(saltBytes);
            }
            return Convert.ToBase64String(saltBytes);
        }

        public static string HashPassword(string password, string salt)
        {
            if (password == null) password = string.Empty;
            if (salt == null) salt = string.Empty;

            using (var sha256 = SHA256.Create())
            {
                byte[] combinedBytes = Encoding.UTF8.GetBytes(salt + ":" + password);
                byte[] hashBytes = sha256.ComputeHash(combinedBytes);
                return BitConverter.ToString(hashBytes).Replace("-", "").ToLowerInvariant();
            }
        }

        public static bool VerifyPassword(string password, string salt, string expectedHash)
        {
            if (string.IsNullOrEmpty(expectedHash)) return false;
            string actualHash = HashPassword(password, salt);
            return string.Equals(actualHash, expectedHash, StringComparison.OrdinalIgnoreCase);
        }
    }
}
