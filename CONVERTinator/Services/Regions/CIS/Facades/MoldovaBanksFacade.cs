using CONVERTinator.Domain;
using CONVERTinator.Helpers;
using CONVERTinator.Services.Regions.CIS.Providers.Moldova;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CONVERTinator.Services.Regions.Facades
{
    internal class MoldovaBanksFacade : IExchangeRateProvider
    {
        private readonly List<IExchangeRateProvider> _localBanks;

        public MoldovaBanksFacade()
        {
            _localBanks = new List<IExchangeRateProvider>
            {
                new BnmProvider() // National Bank of Moldova (Base: MDL)
            };
        }

        public async Task<List<Currency>> GetRatesAsync()
        {
            var tasks = _localBanks.Select(async bank =>
            {
                try { return await bank.GetRatesAsync(); }
                catch (Exception ex)
                {
                    Console.WriteLine($"[MOLDOVA FACADE WARN] Provider failed: {ex.Message}");
                    return new List<Currency>();
                }
            });

            var results = await Task.WhenAll(tasks);
            var validRates = results.Where(list => list != null).SelectMany(list => list).ToList();

            if (validRates.Count == 0) return new List<Currency>();

            // Search course in leu
            var usdRateObj = validRates.FirstOrDefault(c => c.Code == "USD");
            if (usdRateObj == null) return new List<Currency>();

            decimal mdlToUsdCrossRate = usdRateObj.Value;

            var allNormalizedRates = new List<Currency>();

            //  N.O.R.M.A.
            foreach (var rate in validRates)
            {
                if (rate.Code == "USD") continue;

                // Use LocalBase, becouse local currency is MDL.
                var cleanRate = RateNormalizer.NormalizeLocalBase(rate, mdlToUsdCrossRate);
                if (cleanRate != null)
                {
                    allNormalizedRates.Add(cleanRate);
                }
            }

            allNormalizedRates.Add(new Currency
            {
                Code = "MDL",
                Name = "Moldovan Leu",
                Value = mdlToUsdCrossRate,
                Source = "Moldova Facade"
            });

            return allNormalizedRates;
        }
    }
}