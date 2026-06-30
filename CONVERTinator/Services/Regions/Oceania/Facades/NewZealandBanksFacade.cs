using CONVERTinator.Domain.Entities;
using CONVERTinator.Domain.Interfaces;
using CONVERTinator.Helpers;
using CONVERTinator.Services.Providers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CONVERTinator.Services.Regions.Oceania.Facades
{
    internal class NewZealandBanksFacade : IExchangeRateProvider
    {
        private readonly List<IExchangeRateProvider> _localBanks;

        public NewZealandBanksFacade()
        {
            _localBanks = new List<IExchangeRateProvider>
            {
                // Universal provider configured for New Zealand Dollar (NZD)
                new RegionalFloatRatesProvider("nzd", "Reserve Bank of New Zealand (via FR)")
            };
        }

        public async Task<List<Currency>> GetRatesAsync()
        {
            var tasks = _localBanks.Select(async bank =>
            {
                try { return await bank.GetRatesAsync(); }
                catch (Exception ex)
                {
                    Console.WriteLine($"[NEW ZEALAND FACADE WARN] Provider failed: {ex.Message}");
                    return new List<Currency>();
                }
            });

            var results = await Task.WhenAll(tasks);
            var validRates = results.Where(list => list != null).SelectMany(list => list).ToList();

            if (validRates.Count == 0) return new List<Currency>();

            var usdRateObj = validRates.FirstOrDefault(c => c.Code == "USD");
            if (usdRateObj == null) return new List<Currency>();

            decimal nzdToUsdCrossRate = usdRateObj.Value;

            var allNormalizedRates = new List<Currency>();

            foreach (var rate in validRates)
            {
                if (rate.Code == "USD") continue;

                var cleanRate = RateNormalizer.NormalizeForeignBase(rate, nzdToUsdCrossRate);
                if (cleanRate != null) allNormalizedRates.Add(cleanRate);
            }

            allNormalizedRates.Add(new Currency
            {
                Code = "NZD",
                Name = "New Zealand Dollar",
                Value = 1m / nzdToUsdCrossRate,
                Source = "New Zealand Facade"
            });

            return allNormalizedRates;
        }
    }
}