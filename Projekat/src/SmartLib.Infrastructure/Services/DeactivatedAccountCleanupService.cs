using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SmartLib.Core.Interfaces;

namespace SmartLib.Infrastructure.Services
{
    public sealed class DeactivatedAccountCleanupService : BackgroundService
    {
        private static readonly TimeSpan RunInterval = TimeSpan.FromHours(12);
        private static readonly TimeSpan InitialDelay = TimeSpan.FromMinutes(5);

        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<DeactivatedAccountCleanupService> _logger;

        public DeactivatedAccountCleanupService(
            IServiceScopeFactory scopeFactory,
            ILogger<DeactivatedAccountCleanupService> logger)
        {
            _scopeFactory = scopeFactory;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            try
            {
                await Task.Delay(InitialDelay, stoppingToken);
            }
            catch (TaskCanceledException)
            {
                return;
            }

            while (!stoppingToken.IsCancellationRequested)
            {
                await PurgeAsync(stoppingToken);

                try
                {
                    await Task.Delay(RunInterval, stoppingToken);
                }
                catch (TaskCanceledException)
                {
                    return;
                }
            }
        }

        private async Task PurgeAsync(CancellationToken stoppingToken)
        {
            try
            {
                using var scope = _scopeFactory.CreateScope();
                var repo = scope.ServiceProvider.GetRequiredService<IKorisnikRepository>();
                var cutoffUtc = DateTime.UtcNow.AddDays(-7);

                var deleted = await repo.DeleteDeactivatedOlderThanAsync(cutoffUtc);
                if (deleted > 0)
                {
                    _logger.LogInformation(
                        "Deleted {Count} deactivated accounts older than 7 days.",
                        deleted);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to purge deactivated accounts.");
            }
        }
    }
}
