using CONVERTinator.Domain;
using CONVERTinator.Helpers;
using CONVERTinator.Services.Providers;
using CONVERTinator.Services.Regions.Europe.Providers.Sweden;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CONVERTinator.Services.Regions.Europe.Facades
{
    internal class SwedenBanksFacade : IExchangeRateProvider
    {
        private readonly List<IExchangeRateProvider> _localBanks;

        public SwedenBanksFacade()
        {
            _localBanks = new List<IExchangeRateProvider>
            {
                new RegionalFloatRatesProvider("sek", "Sveriges Riksbank via FR")
            };
        }

        public async Task<List<Currency>> GetRatesAsync()
        {
            var tasks = _localBanks.Select(async bank =>
            {
                try { return await bank.GetRatesAsync(); }
                catch (Exception ex)
                {
                    Console.WriteLine($"[SWEDEN FACADE WARN] Provider failed: {ex.Message}");
                    return new List<Currency>();
                }
            });

            var results = await Task.WhenAll(tasks);
            var validRates = results.Where(list => list != null).SelectMany(list => list).ToList();

            if (validRates.Count == 0) return new List<Currency>();

            var usdRateObj = validRates.FirstOrDefault(c => c.Code == "USD");
            if (usdRateObj == null) return new List<Currency>();

            decimal sekToUsdCrossRate = usdRateObj.Value;

            var allNormalizedRates = new List<Currency>();

            foreach (var rate in validRates)
            {
                if (rate.Code == "USD") continue;

                var cleanRate = RateNormalizer.NormalizeForeignBase(rate, sekToUsdCrossRate);
                if (cleanRate != null) allNormalizedRates.Add(cleanRate);
            }

            allNormalizedRates.Add(new Currency
            {
                Code = "SEK",
                Name = "Swedish Krona",
                Value = 1m / sekToUsdCrossRate,
                Source = "Sweden Facade"
            });

            return allNormalizedRates;
        }
    }
}