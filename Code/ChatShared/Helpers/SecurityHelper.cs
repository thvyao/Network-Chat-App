using System;
using System.Security.Cryptography;
using System.Text;

namespace ChatShared.Helpers
{
    public static class SecurityHelper
    {
        private const int Pbkdf2Iterations = 210000;
        private const int DerivedKeySize = 32;
        public static string GenerateSalt(int size = 16)
        {
            byte[] saltBytes = new byte[size];
            RandomNumberGenerator.Fill(saltBytes);
            return Convert.ToBase64String(saltBytes);
        }
        public static string HashPassword(string password, string salt)
        {
            if (password == null) password = string.Empty;
            if (salt == null) salt = string.Empty;

            byte[] saltBytes = Convert.FromBase64String(salt);
            using (var deriveBytes = new Rfc2898DeriveBytes(password, saltBytes, Pbkdf2Iterations, HashAlgorithmName.SHA256))
            {
                return "PBKDF2$" + Pbkdf2Iterations + "$" + Convert.ToBase64String(deriveBytes.GetBytes(DerivedKeySize));
            }
        }
        public static bool VerifyPassword(string password, string salt, string expectedHash)
        {
            if (string.IsNullOrEmpty(expectedHash)) return false;
            try
            {
                if (expectedHash.StartsWith("PBKDF2$", StringComparison.Ordinal))
                {
                    string[] parts = expectedHash.Split('$');
                    if (parts.Length != 3 || !int.TryParse(parts[1], out int iterations) || iterations < 100000)
                        return false;

                    byte[] expected = Convert.FromBase64String(parts[2]);
                    byte[] saltBytes = Convert.FromBase64String(salt);
                    using (var deriveBytes = new Rfc2898DeriveBytes(password ?? string.Empty, saltBytes, iterations, HashAlgorithmName.SHA256))
                    {
                        return CryptographicOperations.FixedTimeEquals(deriveBytes.GetBytes(expected.Length), expected);
                    }
                }

                // Compatibility with accounts written before the .NET 8 migration.
                using (var sha256 = SHA256.Create())
                {
                    byte[] legacyBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes((salt ?? string.Empty) + ":" + (password ?? string.Empty)));
                    string legacyHash = BitConverter.ToString(legacyBytes).Replace("-", "").ToLowerInvariant();
                    return CryptographicOperations.FixedTimeEquals(Encoding.UTF8.GetBytes(legacyHash), Encoding.UTF8.GetBytes(expectedHash));
                }
            }
            catch (FormatException)
            {
                return false;
            }
        }

        public static bool IsCurrentPasswordHash(string hash) =>
            !string.IsNullOrEmpty(hash) && hash.StartsWith("PBKDF2$", StringComparison.Ordinal);
    }
}
