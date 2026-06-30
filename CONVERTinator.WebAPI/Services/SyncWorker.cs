using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using CONVERTinator.Domain.Interfaces;
using CONVERTinator.Domain;
using System.Reflection.Metadata;

namespace CONVERTinator.WebAPI
{
    // The background timer that runs continuously in the WebAPI host
    public class SyncWorker : BackgroundService
    {
        private readonly ILogger<SyncWorker> _logger;
        private readonly IServiceScopeFactory _serviceScopeFactory;

        public SyncWorker(ILogger<SyncWorker> logger, IServiceScopeFactory serviceScopeFactory)
        {
            _logger = logger;
            _serviceScopeFactory = serviceScopeFactory;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("[Sync Worker] Background timer initialized and running.");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    using (var scope = _serviceScopeFactory.CreateScope())
                    {
                        var cacheSyncService = scope.ServiceProvider.GetRequiredService<ICacheSyncService>();
                        await cacheSyncService.ForceUpdateAsync();
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError($"[Sync Worker] CRITICAL ERROR: {ex.Message}");
                }

                _logger.LogInformation("[Sync Worker] Sleeping for 2 hours...");

                // Suspend execution for 2 hours
                await Task.Delay(TimeSpan.FromHours(Constants.Cache.CacheExpirationHours), stoppingToken);
            }
        }
    }
}