using Blocked_Countries_API.Models;

namespace Blocked_Countries_API.Services
{
    public class BlockedAttemptsLogService
    {
        private readonly List<BlockedAttempt> _attempts = new();
        public void LogAttempt(string ip, string countryCode, bool isBlocked, string userAgent)
        {
            _attempts.Add(new BlockedAttempt
            {
                IPAddress = ip,
                Timestamp = DateTime.UtcNow,
                CountryCode = countryCode,
                BlockedStatus = isBlocked,
                UserAgent = userAgent
            });
        }
        public IEnumerable<BlockedAttempt> GetAttempts(int page, int pageSize)
        {
            return _attempts.Skip((page - 1) * pageSize).Take(pageSize);
        }
    }
}
