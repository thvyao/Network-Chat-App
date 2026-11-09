using System.Collections.Generic;

namespace ChatServer.Infrastructure
{
    public interface IAccountRepository
    {
        string StoragePath { get; }
        IReadOnlyList<UserAccount> Load();
        void Save(IEnumerable<UserAccount> accounts);
    }
}
