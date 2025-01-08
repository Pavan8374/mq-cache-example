using System.Collections.Concurrent;

namespace Feed.API.Utility.Analytics
{
    public class UserTracker
    {
        private readonly ConcurrentDictionary<string, DateTime> _activeUsers = new();
        private readonly Timer _cleanupTimer;
        private readonly TimeSpan _timeout = TimeSpan.FromMinutes(5); // Configurable timeout

        public UserTracker()
        {
            _cleanupTimer = new Timer(CleanupInactiveSessions, null, TimeSpan.Zero, TimeSpan.FromMinutes(1));   
        }

        public void UserActivity(string userId)
        {
            _activeUsers.AddOrUpdate(userId, DateTime.UtcNow, (_, __) => DateTime.UtcNow);
        }

        public int GetConcurrentUsers()
        {
            return _activeUsers.Count;
        }
        private void CleanupInactiveSessions(object state)
        {
            var cutoff = DateTime.UtcNow.Subtract(_timeout);
            var inactiveUsers = _activeUsers.Where(x => x.Value < cutoff).Select(x => x.Key).ToList();

            foreach (var userId in inactiveUsers)
            {
                _activeUsers.TryRemove(userId, out _);
            }
        }

        public void Dispose()
        {
            _cleanupTimer?.Dispose();
        }


    }
}
