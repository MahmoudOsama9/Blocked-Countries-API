using Blocked_Countries_API.Models;
using Blocked_Countries_API.Services;
using Microsoft.AspNetCore.Mvc;

namespace Blocked_Countries_API.Controllers
{
    [Route("api/countries")]
    [ApiController]
    public class BlockedCountriesController : ControllerBase
    {
        private readonly BlockedCountryService _service;
        public BlockedCountriesController(BlockedCountryService service) => _service = service;

        [HttpPost("block")]
        public IActionResult BlockCountry([FromBody] BlockRequest request) =>
            _service.BlockCountry(request.CountryCode) ? Ok() : Conflict("Country already blocked");

        [HttpPost("temporal-block")]
        public IActionResult BlockTemporarily([FromBody] TemporaryBlockRequest request)
        {
            if (request.DurationMinutes < 1 || request.DurationMinutes > 1440)
                return BadRequest("Duration must be between 1 and 1440 minutes.");
            return _service.BlockTemporarily(request.CountryCode, request.DurationMinutes) ? Ok() : Conflict("Country already temporarily blocked");
        }

        [HttpGet("blocked")]
        public IActionResult GetBlockedCountries([FromQuery] int page = 1, [FromQuery] int pageSize = 10, [FromQuery] string? filter = null)
        {
            return Ok(_service.GetBlockedCountries(page, pageSize, filter));
        }

        [HttpDelete("block/{countryCode}")]
        public IActionResult UnblockCountry(string countryCode) =>
            _service.UnblockCountry(countryCode) ? Ok() : NotFound("Country not found");
    }

}
