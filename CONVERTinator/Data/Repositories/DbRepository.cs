using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using CONVERTinator.Domain;
using CONVERTinator.Domain.Entities;
using CONVERTinator.Data;

namespace CONVERTinator.Repositories
{
    public class DbRepository
    {
        private readonly DbContextOptions<AppDbContext> _options;

        // Default constructor for the real application (production mode)
        public DbRepository()
        {
        }

        // Constructor for testing (accepts in-memory database options)
        public DbRepository(DbContextOptions<AppDbContext> options)
        {
            _options = options;
        }

        // Factory method: decides which database context to provide
        private AppDbContext CreateContext()
        {
            return _options != null ? new AppDbContext(_options) : new AppDbContext();
        }

        /// <summary>
        /// Read cached rates from the database
        /// </summary>
        public async Task<List<Currency>> GetCachedRatesAsync()
        {
            using var db = CreateContext();
            var cached = await db.CachedRates.ToListAsync();
            // Map CachedRate entities to Currency domain models
            return cached.Select(c => new Currency
            {
                Code = c.CurrencyCode,
                Value = c.UsdRate,
                Source = c.Source
            }).ToList();
        }

        /// <summary>
        /// Overwrites the cached rates
        /// </summary>
        public async Task SaveRatesAsync(List<Currency> rates)
        {
            using var db = CreateContext();
            var oldRates = await db.CachedRates.ToListAsync();
            db.CachedRates.RemoveRange(oldRates);

            // Prepare new rates
            var newRates = rates.Select(r => new CachedRate
            {
                CurrencyCode = r.Code,
                UsdRate = r.Value,
                Source = r.Source,
                FetchTime = DateTime.UtcNow
            }).ToList();

            db.CachedRates.AddRange(newRates);
            await db.SaveChangesAsync();
        }

        /// <summary>
        /// Checks if the cache is fresh
        /// </summary>
        public async Task<bool> IsCacheFreshAsync(TimeSpan maxAge)
        {
            using var db = CreateContext();
            if (!await db.CachedRates.AnyAsync()) return false;

            DateTime oldestCacheTime = await db.CachedRates.MinAsync(r => r.FetchTime);
            return (DateTime.UtcNow - oldestCacheTime) < maxAge;
        }

        /// <summary>
        /// Gets the user settings
        /// </summary>
        public async Task<UserSettings> GetSettingsAsync()
        {
            using var db = CreateContext();
            var settings = await db.Settings.FirstOrDefaultAsync();

            if (settings == null)
            {
                settings = new UserSettings
                {
                    BaseCurrency = "USD",
                    SavedCurrencies = "EUR"
                };
                db.Settings.Add(settings);
                await db.SaveChangesAsync();
            }
            return settings;
        }

        /// <summary>
        /// Saves the user settings
        /// </summary>
        public async Task SaveSettingsAsync(string baseCurrency, List<string> activeCurrencies)
        {
            using var db = CreateContext();
            var settings = await db.Settings.FirstOrDefaultAsync();

            if (settings == null)
            {
                settings = new UserSettings();
                db.Settings.Add(settings);
            }

            settings.BaseCurrency = baseCurrency;
            settings.SavedCurrencies = string.Join(",", activeCurrencies.Select(c => c.Trim().ToUpper()));
            await db.SaveChangesAsync();
        }
    }
}