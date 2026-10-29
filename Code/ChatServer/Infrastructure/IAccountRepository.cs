using System.Collections.Generic;

namespace ChatServer.Infrastructure
{
    /// <summary>Abstraction for account persistence; replace it to use SQLite or a remote database.</summary>
    public interface IAccountRepository
    {
        IReadOnlyList<UserAccount> Load();
        void Save(IEnumerable<UserAccount> accounts);
    }
}
