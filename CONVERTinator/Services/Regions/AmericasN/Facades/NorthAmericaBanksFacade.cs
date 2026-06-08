using CONVERTinator.Domain;
using CONVERTinator.Helpers;
using CONVERTinator.Services.Regions.AmericasN.Providers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CONVERTinator.Services.Regions.Facades
{
    internal class NorthAmericaBanksFacade : IExchangeRateProvider
    {
        private readonly List<IExchangeRateProvider> _localBanks;

        public NorthAmericaBanksFacade()
        {
            _localBanks = new List<IExchangeRateProvider>
            {
                new UsProvider(),
                new BankOfCanadaProvider()
            };
        }

        public async Task<List<Currency>> GetRatesAsync()
        {
            var tasks = _localBanks.Select(async bank =>
            {
                try { return await bank.GetRatesAsync(); }
                catch (Exception ex)
                {
                    Console.WriteLine($"[NORTH AMERICA FACADE WARN] Provider failed: {ex.Message}");
                    return new List<Currency>();
                }
            });

            var results = await Task.WhenAll(tasks);
            var validRates = results.Where(list => list != null).SelectMany(list => list).ToList();

            if (validRates.Count == 0) return new List<Currency>();

            var usdRateObj = validRates.FirstOrDefault(c => c.Code == "USD" && c.Source == "Bank of Canada");
            decimal cadToUsdCrossRate = usdRateObj?.Value ?? 1.35m; // Fallback parity if missing

            var allNormalizedRates = new List<Currency>();

            // Route the data through appropriate mathematical algorithms based on origin
            foreach (var rate in validRates)
            {
                if (rate.Code == "USD") continue;

                if (rate.Source == "US Global API")
                {
                    // Bypassing N.O.R.M.A.
                    allNormalizedRates.Add(rate);
                }
                else if (rate.Source == "Bank of Canada")
                {
                    
                    var cleanRate = RateNormalizer.NormalizeLocalBase(rate, cadToUsdCrossRate);
                    if (cleanRate != null) allNormalizedRates.Add(cleanRate);
                }
            }

            allNormalizedRates.Add(new Currency { Code = "CAD", Name = "Canadian Dollar", Value = cadToUsdCrossRate, Source = "North America Facade" });
            allNormalizedRates.Add(new Currency { Code = "USD", Name = "United States Dollar", Value = 1m, Source = "North America Facade" });

            // Data Consensus
            var finalConsensusRates = allNormalizedRates
                .GroupBy(c => c.Code)
                .Select(group => new Currency
                {
                    Code = group.Key,
                    Name = group.First().Name,
                    Value = group.Average(c => c.Value),
                    Source = "North America Facade (Consensus)"
                })
                .ToList();

            return finalConsensusRates;
        }
    }
}