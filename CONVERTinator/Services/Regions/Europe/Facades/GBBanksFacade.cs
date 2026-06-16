using CONVERTinator.Domain;
using CONVERTinator.Helpers;
using CONVERTinator.Services.Providers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CONVERTinator.Services.Regions.Europe.Facades
{
    internal class GBBanksFacade : IExchangeRateProvider
    {
        private readonly List<IExchangeRateProvider> _localBanks;

        public GBBanksFacade()
        {
            _localBanks = new List<IExchangeRateProvider>
            {
                // Universal provider configured for British Pound (GBP)
                new RegionalFloatRatesProvider("gbp", "Bank of England (via FR)")
            };
        }

        public async Task<List<Currency>> GetRatesAsync()
        {
            var tasks = _localBanks.Select(async bank =>
            {
                try { return await bank.GetRatesAsync(); }
                catch (Exception ex)
                {
                    Console.WriteLine($"[GB FACADE WARN] Provider failed: {ex.Message}");
                    return new List<Currency>();
                }
            });

            var results = await Task.WhenAll(tasks);
            var validRates = results.Where(list => list != null).SelectMany(list => list).ToList();

            if (validRates.Count == 0) return new List<Currency>();

            var usdRateObj = validRates.FirstOrDefault(c => c.Code == "USD");
            if (usdRateObj == null) return new List<Currency>();

            decimal gbpToUsdCrossRate = usdRateObj.Value;

            var allNormalizedRates = new List<Currency>();

            foreach (var rate in validRates)
            {
                if (rate.Code == "USD") continue;

                var cleanRate = RateNormalizer.NormalizeForeignBase(rate, gbpToUsdCrossRate);
                if (cleanRate != null) allNormalizedRates.Add(cleanRate);
            }

            allNormalizedRates.Add(new Currency
            {
                Code = "GBP",
                Name = "British Pound",
                Value = 1m / gbpToUsdCrossRate,
                Source = "GB Facade"
            });

            return allNormalizedRates;
        }
    }
}