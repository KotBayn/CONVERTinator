using CONVERTinator.Domain;
using CONVERTinator.Helpers;
using CONVERTinator.Services.Regions.Europe.Providers.Romania;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CONVERTinator.Services.Regions.Europe.Facades
{
    internal class RomaniaBanksFacade : IExchangeRateProvider
    {
        private readonly List<IExchangeRateProvider> _localBanks;

        public RomaniaBanksFacade()
        {
            _localBanks = new List<IExchangeRateProvider>
            {
                new BnrProvider() // National Bank of Romania (Base: RON)
            };
        }

        public async Task<List<Currency>> GetRatesAsync()
        {
            var tasks = _localBanks.Select(async bank =>
            {
                try { return await bank.GetRatesAsync(); }
                catch (Exception ex)
                {
                    Console.WriteLine($"[ROMANIA FACADE WARN] Provider failed: {ex.Message}");
                    return new List<Currency>();
                }
            });

            var results = await Task.WhenAll(tasks);
            var validRates = results.Where(list => list != null).SelectMany(list => list).ToList();

            if (validRates.Count == 0) return new List<Currency>();

            // Establish the anchor rate (USD value in RON)
            var usdRateObj = validRates.FirstOrDefault(c => c.Code == "USD");
            if (usdRateObj == null) return new List<Currency>();

            decimal ronToUsdCrossRate = usdRateObj.Value;

            var allNormalizedRates = new List<Currency>();

            //  N.O.R.M.A. 
            foreach (var rate in validRates)
            {
                if (rate.Code == "USD") continue;

                var cleanRate = RateNormalizer.NormalizeLocalBase(rate, ronToUsdCrossRate);
                if (cleanRate != null)
                {
                    allNormalizedRates.Add(cleanRate);
                }
            }

            // Inject the Romanian Leu (RON)
            allNormalizedRates.Add(new Currency
            {
                Code = "RON",
                Name = "Romanian Leu",
                Value = ronToUsdCrossRate, // NO DIVISION. Pure cross-rate value.
                Source = "Romania Facade"
            });

            return allNormalizedRates;
        }
    }
}