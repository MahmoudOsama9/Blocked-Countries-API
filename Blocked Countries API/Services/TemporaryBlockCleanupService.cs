namespace Blocked_Countries_API.Services
{
    public class TemporaryBlockCleanupService : BackgroundService
    {
        private readonly BlockedCountryService _service;
        public TemporaryBlockCleanupService(BlockedCountryService service) => _service = service;
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                _service.CleanupTemporaryBlocks();
                await Task.Delay(TimeSpan.FromMinutes(5), stoppingToken);
            }
        }
    }
}
