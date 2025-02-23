namespace Blocked_Countries_API.Models
{
    public class BlockedAttempt
    {
        public string IPAddress { get; set; }
        public DateTime Timestamp { get; set; }
        public string CountryCode { get; set; }
        public bool BlockedStatus { get; set; }
        public string UserAgent { get; set; }
    }
}
