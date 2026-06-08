using CONVERTinator.Domain;
using CONVERTinator.Helpers;
using CONVERTinator.Services.Regions.Europe.Providers.Czech;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CONVERTinator.Services.Regions.Europe.Facades
{
    internal class CzechBanksFacade : IExchangeRateProvider
    {
        private readonly List<IExchangeRateProvider> _localBanks;

        public CzechBanksFacade()
        {
            _localBanks = new List<IExchangeRateProvider>
            {
                new CnbProvider() // Returns rates based in CZK
            };
        }

        public async Task<List<Currency>> GetRatesAsync()
        {
            var tasks = _localBanks.Select(async bank =>
            {
                try { return await bank.GetRatesAsync(); }
                catch (Exception ex)
                {
                    Console.WriteLine($"[CZECH FACADE WARN] Provider failed: {ex.Message}");
                    return new List<Currency>();
                }
            });

            var results = await Task.WhenAll(tasks);
            var validRates = results.Where(list => list != null).SelectMany(list => list).ToList();

            if (validRates.Count == 0) return new List<Currency>();

            // Establish the anchor rate (USD value in CZK)
            var usdRateObj = validRates.FirstOrDefault(c => c.Code == "USD");
            if (usdRateObj == null) return new List<Currency>();

            decimal czkToUsdCrossRate = usdRateObj.Value;

            var allNormalizedRates = new List<Currency>();

            // N.O.R.M.A.
            foreach (var rate in validRates)
            {
                if (rate.Code == "USD") continue;

                var cleanRate = RateNormalizer.NormalizeLocalBase(rate, czkToUsdCrossRate);
                if (cleanRate != null)
                {
                    allNormalizedRates.Add(cleanRate);
                }
            }

            // Explicitly inject the Czech Koruna (CZK)
            allNormalizedRates.Add(new Currency
            {
                Code = "CZK",
                Name = "Czech Koruna",
                Value = czkToUsdCrossRate, // NO DIVISION! Direct insertion.
                Source = "Czech Facade"
            });

            return allNormalizedRates;
        }
    }
}