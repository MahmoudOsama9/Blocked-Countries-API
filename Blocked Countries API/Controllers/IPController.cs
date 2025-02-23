using Blocked_Countries_API.Models;
using Blocked_Countries_API.Services;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace Blocked_Countries_API.Controllers
{
    [Route("api/ip")]
    [ApiController]
    public class IPController : ControllerBase
    {
        private readonly HttpClient _httpClient;
        private readonly BlockedCountryService _blockedCountryService;
        private readonly BlockedAttemptsLogService _logService;
        private readonly string _geoApiKey;
        private readonly string _geoBaseUrl;
        public IPController(HttpClient httpClient, IConfiguration configuration, BlockedCountryService blockedCountryService, BlockedAttemptsLogService logService)
        {
            _httpClient = httpClient;
            _blockedCountryService = blockedCountryService;
            _logService = logService;
            _geoApiKey = configuration["GeoLocation:ApiKey"];
            _geoBaseUrl = configuration["GeoLocation:BaseUrl"];
        }
        [HttpGet("lookup")]
        public async Task<IActionResult> LookupIP([FromQuery] string? ipAddress = null)
        {
            if (string.IsNullOrWhiteSpace(ipAddress))
            {
                ipAddress = Request.Headers["X-Forwarded-For"].FirstOrDefault()
                            ?? Request.Headers["CF-Connecting-IP"].FirstOrDefault()
                            ?? HttpContext.Connection.RemoteIpAddress?.ToString();
            }

            if (string.IsNullOrWhiteSpace(ipAddress) || !IsValidIPAddress(ipAddress))
                return BadRequest("Invalid or missing IP address.");

            var response = await _httpClient.GetAsync($"{_geoBaseUrl}/{ipAddress}?apiKey={_geoApiKey}");
            if (!response.IsSuccessStatusCode)
                return StatusCode((int)response.StatusCode, "Failed to fetch IP details.");

            var content = await response.Content.ReadAsStringAsync();
            var ipData = JsonSerializer.Deserialize<IPResponse>(content);

            if (ipData == null || string.IsNullOrEmpty(ipData.CountryCode))
                return BadRequest("Invalid response from IP lookup service.");

            return Ok(ipData);
        }
        [HttpGet("check-block")]
        public async Task<IActionResult> CheckIfBlocked()
        {
            string? callerIP = HttpContext.Connection.RemoteIpAddress?.ToString();

            if (string.IsNullOrWhiteSpace(callerIP) || !IsValidIPAddress(callerIP))
                return BadRequest("Unable to determine caller IP.");

            var response = await _httpClient.GetAsync($"{_geoBaseUrl}/{callerIP}?apiKey={_geoApiKey}");
            if (!response.IsSuccessStatusCode)
                return StatusCode((int)response.StatusCode, "Failed to fetch IP details.");

            var content = await response.Content.ReadAsStringAsync();
            var ipData = JsonSerializer.Deserialize<IPResponse>(content);

            if (ipData == null || string.IsNullOrEmpty(ipData.CountryCode))
                return BadRequest("Invalid response from IP lookup service.");

            bool isBlocked = _blockedCountryService.IsCountryBlocked(ipData.CountryCode);
            _logService.LogAttempt(callerIP, ipData.CountryCode, isBlocked, Request.Headers["User-Agent"].ToString());

            return Ok(new
            {
                IP = callerIP,
                Country = ipData.CountryName,
                CountryCode = ipData.CountryCode,
                Blocked = isBlocked
            });
        }
        private static bool IsValidIPAddress(string ip)
        {
            return Regex.IsMatch(ip, @"^(?:\d{1,3}\.){3}\d{1,3}$|^([a-fA-F0-9]{1,4}:){7}[a-fA-F0-9]{1,4}$");
        }
    }

}
