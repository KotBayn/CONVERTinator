using CONVERTinator.Domain;
using CONVERTinator.Helpers;
using CONVERTinator.Services.Providers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CONVERTinator.Services.Regions.Asia.Facades
{
    internal class IndiaBanksFacade : IExchangeRateProvider
    {
        private readonly List<IExchangeRateProvider> _localBanks;

        public IndiaBanksFacade()
        {
            _localBanks = new List<IExchangeRateProvider>
            {
                // Using universal provider configured specifically for India
                new RegionalFloatRatesProvider("inr", "Reserve Bank of India (via FloatRates)")
            };
        }

        public async Task<List<Currency>> GetRatesAsync()
        {
            var tasks = _localBanks.Select(async bank =>
            {
                try { return await bank.GetRatesAsync(); }
                catch (Exception ex)
                {
                    Console.WriteLine($"[INDIA FACADE WARN] Provider failed: {ex.Message}");
                    return new List<Currency>();
                }
            });

            var results = await Task.WhenAll(tasks);
            var validRates = results.Where(list => list != null).SelectMany(list => list).ToList();

            if (validRates.Count == 0) return new List<Currency>();

            // Anchor: How many INR for 1 USD
            var usdRateObj = validRates.FirstOrDefault(c => c.Code == "USD");
            if (usdRateObj == null) return new List<Currency>();

            decimal inrToUsdCrossRate = usdRateObj.Value;
            var allNormalizedRates = new List<Currency>();

            // N.O.R.M.A.
            foreach (var rate in validRates)
            {
                if (rate.Code == "USD") continue;

                var cleanRate = RateNormalizer.NormalizeForeignBase(rate, inrToUsdCrossRate);
                if (cleanRate != null) allNormalizedRates.Add(cleanRate);
            }

            // Inject Indian Rupee (INR) explicitly
            allNormalizedRates.Add(new Currency
            {
                Code = "INR",
                Name = "Indian Rupee",
                Value = 1m / inrToUsdCrossRate,
                Source = "India Facade"
            });

            return allNormalizedRates;
        }
    }
}