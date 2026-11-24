using System;
using System.Collections.Concurrent;

namespace ChatServer
{
    internal sealed class LoginAttemptLimiter
    {
        private const int MaximumFailures = 5;
        private static readonly TimeSpan LockDuration = TimeSpan.FromMinutes(5);
        private readonly ConcurrentDictionary<string, AttemptState> attempts = new ConcurrentDictionary<string, AttemptState>();

        public bool TryAllow(string username, string remoteAddress, out string errorMessage)
        {
            string key = CreateKey(username, remoteAddress);
            errorMessage = null;
            if (!attempts.TryGetValue(key, out AttemptState state)) return true;

            lock (state)
            {
                if (state.LockedUntil > DateTime.UtcNow)
                {
                    errorMessage = "Đã có quá nhiều lần đăng nhập không thành công. Vui lòng thử lại sau vài phút.";
                    return false;
                }
                if (state.LockedUntil.HasValue) attempts.TryRemove(key, out _);
                return true;
            }
        }

        public void RecordFailure(string username, string remoteAddress)
        {
            string key = CreateKey(username, remoteAddress);
            AttemptState state = attempts.GetOrAdd(key, _ => new AttemptState());
            lock (state)
            {
                state.FailureCount++;
                if (state.FailureCount >= MaximumFailures)
                    state.LockedUntil = DateTime.UtcNow.Add(LockDuration);
            }
        }

        public void Clear(string username, string remoteAddress) =>
            attempts.TryRemove(CreateKey(username, remoteAddress), out _);

        private static string CreateKey(string username, string remoteAddress) =>
            (username ?? string.Empty).Trim().ToUpperInvariant() + "|" + (remoteAddress ?? string.Empty);

        private sealed class AttemptState
        {
            public int FailureCount { get; set; }
            public DateTime? LockedUntil { get; set; }
        }
    }
}
