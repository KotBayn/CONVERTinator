using CONVERTinator.Domain.Entities;
using CONVERTinator.Domain.Interfaces;
using CONVERTinator.Helpers;
using CONVERTinator.Services.Providers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CONVERTinator.Services.Regions.Europe.Facades
{
    internal class SwitzerlandBanksFacade : IExchangeRateProvider
    {
        private readonly List<IExchangeRateProvider> _localBanks;

        public SwitzerlandBanksFacade()
        {
            _localBanks = new List<IExchangeRateProvider>
            {
                // Universal provider configured for Swiss Franc (CHF)
                new RegionalFloatRatesProvider("chf", "Swiss National Bank (via FR)")
            };
        }

        public async Task<List<Currency>> GetRatesAsync()
        {
            var tasks = _localBanks.Select(async bank =>
            {
                try { return await bank.GetRatesAsync(); }
                catch (Exception ex)
                {
                    Console.WriteLine($"[SWITZERLAND FACADE WARN] Provider failed: {ex.Message}");
                    return new List<Currency>();
                }
            });

            var results = await Task.WhenAll(tasks);
            var validRates = results.Where(list => list != null).SelectMany(list => list).ToList();

            if (validRates.Count == 0) return new List<Currency>();

            var usdRateObj = validRates.FirstOrDefault(c => c.Code == "USD");
            if (usdRateObj == null) return new List<Currency>();

            decimal chfToUsdCrossRate = usdRateObj.Value;

            var allNormalizedRates = new List<Currency>();

            foreach (var rate in validRates)
            {
                if (rate.Code == "USD") continue;

                var cleanRate = RateNormalizer.NormalizeForeignBase(rate, chfToUsdCrossRate);
                if (cleanRate != null) allNormalizedRates.Add(cleanRate);
            }

            allNormalizedRates.Add(new Currency
            {
                Code = "CHF",
                Name = "Swiss Franc",
                Value = 1m / chfToUsdCrossRate,
                Source = "Switzerland Facade"
            });

            return allNormalizedRates;
        }
    }
}