using CONVERTinator.Domain;
using CONVERTinator.Helpers;
using CONVERTinator.Services.Providers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CONVERTinator.Services.Regions.Europe.Facades
{
    internal class DenmarkBanksFacade : IExchangeRateProvider
    {
        private readonly List<IExchangeRateProvider> _localBanks;

        public DenmarkBanksFacade()
        {
            _localBanks = new List<IExchangeRateProvider>
            {
                new RegionalFloatRatesProvider("dkk", "Danmarks Nationalbank via FR")
            };
        }

        public async Task<List<Currency>> GetRatesAsync()
        {
            var tasks = _localBanks.Select(async bank =>
            {
                try { return await bank.GetRatesAsync(); }
                catch (Exception ex)
                {
                    Console.WriteLine($"[DENMARK FACADE WARN] Provider failed: {ex.Message}");
                    return new List<Currency>();
                }
            });

            var results = await Task.WhenAll(tasks);
            var validRates = results.Where(list => list != null).SelectMany(list => list).ToList();

            if (validRates.Count == 0) return new List<Currency>();

            var usdRateObj = validRates.FirstOrDefault(c => c.Code == "USD");
            if (usdRateObj == null) return new List<Currency>();

            decimal dkkToUsdCrossRate = usdRateObj.Value;

            var allNormalizedRates = new List<Currency>();

            foreach (var rate in validRates)
            {
                if (rate.Code == "USD") continue;

                var cleanRate = RateNormalizer.NormalizeForeignBase(rate, dkkToUsdCrossRate);
                if (cleanRate != null) allNormalizedRates.Add(cleanRate);
            }

            allNormalizedRates.Add(new Currency
            {
                Code = "DKK",
                Name = "Danish Krone",
                Value = 1m / dkkToUsdCrossRate,
                Source = "Denmark Facade"
            });

            return allNormalizedRates;
        }
    }
}