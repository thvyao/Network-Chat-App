using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;

namespace ChatServer.Infrastructure
{
    public sealed class TextAccountRepository : IAccountRepository
    {
        private readonly string filePath;

        public string StoragePath => filePath;

        public TextAccountRepository(string storageDirectory)
        {
            if (string.IsNullOrWhiteSpace(storageDirectory))
                throw new ArgumentException("Storage directory is required.", nameof(storageDirectory));

            filePath = Path.Combine(storageDirectory, "accounts.txt");
        }

        public IReadOnlyList<UserAccount> Load()
        {
            if (!File.Exists(filePath)) return new List<UserAccount>();

            var accounts = new List<UserAccount>();
            foreach (string line in File.ReadAllLines(filePath, Encoding.UTF8))
            {
                if (string.IsNullOrWhiteSpace(line) || line.StartsWith("#")) continue;

                string[] fields = line.Split('|');
                if (fields.Length != 6) continue;

                try
                {
                    accounts.Add(new UserAccount
                    {
                        Username = fields[0],
                        PasswordHash = fields[1],
                        Salt = fields[2],
                        CreatedAt = DateTime.Parse(fields[3], CultureInfo.InvariantCulture,
                            DateTimeStyles.RoundtripKind),
                        LastLogin = string.IsNullOrEmpty(fields[4])
                            ? (DateTime?)null
                            : DateTime.Parse(fields[4], CultureInfo.InvariantCulture,
                                DateTimeStyles.RoundtripKind),
                        AvatarIndex = int.Parse(fields[5], CultureInfo.InvariantCulture)
                    });
                }
                catch (FormatException)
                {
                }
            }

            return accounts;
        }

        public void Save(IEnumerable<UserAccount> accounts)
        {
            var lines = new List<string>
            {
                "# username|passwordHash|salt|createdAt|lastLogin|avatarIndex"
            };

            lines.AddRange((accounts ?? Enumerable.Empty<UserAccount>())
                .Where(account => account != null)
                .OrderBy(account => account.Username, StringComparer.OrdinalIgnoreCase)
                .Select(account => string.Join("|",
                    account.Username ?? string.Empty,
                    account.PasswordHash ?? string.Empty,
                    account.Salt ?? string.Empty,
                    account.CreatedAt.ToString("o", CultureInfo.InvariantCulture),
                    account.LastLogin.HasValue
                        ? account.LastLogin.Value.ToString("o", CultureInfo.InvariantCulture)
                        : string.Empty,
                    account.AvatarIndex.ToString(CultureInfo.InvariantCulture))));

            File.WriteAllLines(filePath, lines, Encoding.UTF8);
        }
    }
}
