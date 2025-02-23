using System.Collections.Concurrent;

namespace Blocked_Countries_API.Services
{
    public class BlockedCountryService
    {
        private readonly ConcurrentDictionary<string, bool> _blockedCountries = new();
        private readonly ConcurrentDictionary<string, DateTime> _temporaryBlockedCountries = new();

        public bool BlockCountry(string countryCode) => _blockedCountries.TryAdd(countryCode, true);
        public bool UnblockCountry(string countryCode) => _blockedCountries.TryRemove(countryCode, out _);
        public IEnumerable<string> GetBlockedCountries(int page, int pageSize, string? filter)
        {
            var query = _blockedCountries.Keys.AsQueryable();
            if (!string.IsNullOrEmpty(filter))
            {
                query = query.Where(c => c.Contains(filter, StringComparison.OrdinalIgnoreCase));
            }
            return query.Skip((page - 1) * pageSize).Take(pageSize);
        }
        public bool BlockTemporarily(string countryCode, int durationMinutes)
        {
            if (_temporaryBlockedCountries.ContainsKey(countryCode)) return false;
            _temporaryBlockedCountries[countryCode] = DateTime.UtcNow.AddMinutes(durationMinutes);
            return true;
        }
        public bool IsCountryBlocked(string countryCode) => _blockedCountries.ContainsKey(countryCode) ||
                                                             (_temporaryBlockedCountries.TryGetValue(countryCode, out var expiry) && expiry > DateTime.UtcNow);
        public void CleanupTemporaryBlocks()
        {
            foreach (var kv in _temporaryBlockedCountries.Where(kv => kv.Value <= DateTime.UtcNow).ToList())
            {
                _temporaryBlockedCountries.TryRemove(kv.Key, out _);
            }
        }
    }

}
