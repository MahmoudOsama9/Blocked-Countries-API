namespace Blocked_Countries_API.Models
{
    public class TemporaryBlockRequest
    {
        public string CountryCode { get; set; }
        public int DurationMinutes { get; set; }
    }
}
