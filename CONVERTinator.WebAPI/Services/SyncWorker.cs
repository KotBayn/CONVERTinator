using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using CONVERTinator.Services; 

namespace CONVERTinator.WebAPI
{
    // The background timer that runs continuously in the WebAPI host
    public class SyncWorker : BackgroundService
    {
        private readonly ILogger<SyncWorker> _logger;

        public SyncWorker(ILogger<SyncWorker> logger)
        {
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("[Sync Worker] Background timer initialized and running.");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    var cacheSyncService = new CacheSyncService();
                    await cacheSyncService.ForceUpdateAsync();
                }
                catch (Exception ex)
                {
                    _logger.LogError($"[Sync Worker] CRITICAL ERROR: {ex.Message}");
                }

                _logger.LogInformation("[Sync Worker] Sleeping for 2 hours...");

                // Suspend execution for 2 hours
                await Task.Delay(TimeSpan.FromHours(2), stoppingToken);
            }
        }
    }
}