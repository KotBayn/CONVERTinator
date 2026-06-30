using CONVERTinator.Domain.Entities;
using CONVERTinator.Domain.Interfaces;
using CONVERTinator.Helpers;
using CONVERTinator.Services.Providers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CONVERTinator.Services.Regions.Asia.Facades
{
    internal class SouthKoreaBanksFacade : IExchangeRateProvider
    {
        private readonly List<IExchangeRateProvider> _localBanks;

        public SouthKoreaBanksFacade()
        {
            _localBanks = new List<IExchangeRateProvider>
            {
                // Using universal provider configured specifically for South Korea
                new RegionalFloatRatesProvider("krw", "Bank of Korea (via FloatRates)")
            };
        }

        public async Task<List<Currency>> GetRatesAsync()
        {
            var tasks = _localBanks.Select(async bank =>
            {
                try { return await bank.GetRatesAsync(); }
                catch (Exception ex)
                {
                    Console.WriteLine($"[SOUTH KOREA FACADE WARN] Provider failed: {ex.Message}");
                    return new List<Currency>();
                }
            });

            var results = await Task.WhenAll(tasks);
            var validRates = results.Where(list => list != null).SelectMany(list => list).ToList();

            if (validRates.Count == 0) return new List<Currency>();

            // Anchor: How many KRW for 1 USD
            var usdRateObj = validRates.FirstOrDefault(c => c.Code == "USD");
            if (usdRateObj == null) return new List<Currency>();

            decimal krwToUsdCrossRate = usdRateObj.Value;
            var allNormalizedRates = new List<Currency>();

            // N.O.R.M.A.
            foreach (var rate in validRates)
            {
                if (rate.Code == "USD") continue;

                var cleanRate = RateNormalizer.NormalizeForeignBase(rate, krwToUsdCrossRate);
                if (cleanRate != null) allNormalizedRates.Add(cleanRate);
            }

            // Inject South Korean Won (KRW) explicitly
            allNormalizedRates.Add(new Currency
            {
                Code = "KRW",
                Name = "South Korean Won",
                Value = 1m / krwToUsdCrossRate,
                Source = "South Korea Facade"
            });

            return allNormalizedRates;
        }
    }
}