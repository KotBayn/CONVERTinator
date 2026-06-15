using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using CONVERTinator.Data;
using CONVERTinator.Domain;
using CONVERTinator.Domain.Entities;

namespace CONVERTinator.Repositories
{
    public class DbRepository
    {
        /// <summary>
        /// Update the cached currency rates in the database.
        /// Abstraction: converting the business model (Currency) to the database model (CachedRate).
        /// </summary>
        public async Task SaveRatesAsync(List<Currency> fetchedRates)
        {
            // Open a connection to the database
            using var db = new AppDbContext();

            // Fetch all current rates from the database into memory
            var existingRates = await db.CachedRates.ToDictionaryAsync(r => r.CurrencyCode);

            var ratesToAdd = new List<CachedRate>();
            var currentTime = DateTime.UtcNow; // Always store time in UTC!

            // Sort
            foreach (var rate in fetchedRates)
            {
                if (existingRates.TryGetValue(rate.Code, out var dbRate))
                {
                    dbRate.UsdRate = rate.Value;
                    dbRate.FetchTime = currentTime;
                    dbRate.Source = rate.Source;
                }
                else
                {
                    // New currency -> prepare to add
                    ratesToAdd.Add(new CachedRate
                    {
                        CurrencyCode = rate.Code,
                        UsdRate = rate.Value,
                        FetchTime = currentTime,
                        Source = rate.Source
                    });
                }
            }

            if (ratesToAdd.Any())
            {
                await db.CachedRates.AddRangeAsync(ratesToAdd);
            }

            await db.SaveChangesAsync();
        }

        /// <summary>
        /// Fetches the most recent rates from the local database
        /// </summary>
        public async Task<List<Currency>> GetCachedRatesAsync()
        {
            using var db = new AppDbContext();

            var cachedRates = await db.CachedRates.ToListAsync();

            // Convert data from Entity (DB) back to our convenient Currency objects
            return cachedRates.Select(r => new Currency
            {
                Code = r.CurrencyCode,
                Name = r.CurrencyCode, 
                Value = r.UsdRate,
                Source = r.Source + " [CACHED]"
            }).ToList();
        }
        /// <summary>
        /// Check if there is a live cache in the database that is younger than the specified age.
        /// </summary>
        public async Task<bool> IsCacheFreshAsync(TimeSpan maxAge)
        {
            using var db = new AppDbContext();

            // If the table is empty, the cache is invalid
            if (!await db.CachedRates.AnyAsync()) return false;

            // Get the time of the oldest record in the database
            DateTime oldestCacheTime = await db.CachedRates.MinAsync(r => r.FetchTime);

            // If the time since the oldest record is less than maxAge, the cache is fresh
            return (DateTime.UtcNow - oldestCacheTime) < maxAge;
        }
    }
}