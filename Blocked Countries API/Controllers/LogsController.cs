using Blocked_Countries_API.Services;
using Microsoft.AspNetCore.Mvc;

namespace Blocked_Countries_API.Controllers
{
    [Route("api/logs")]
    [ApiController]
    public class LogsController : ControllerBase
    {
        private readonly BlockedAttemptsLogService _logService;
        public LogsController(BlockedAttemptsLogService logService) => _logService = logService;

        [HttpGet("blocked-attempts")]
        public IActionResult GetBlockedAttempts([FromQuery] int page = 1, [FromQuery] int pageSize = 10) =>
            Ok(_logService.GetAttempts(page, pageSize));
    }

}
