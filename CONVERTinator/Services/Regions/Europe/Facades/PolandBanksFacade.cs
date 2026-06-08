using CONVERTinator.Domain;
using CONVERTinator.Helpers;
using CONVERTinator.Services.Regions.Europe.Providers.Poland;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CONVERTinator.Services.Regions.Europe.Facades
{
    internal class PolandBanksFacade : IExchangeRateProvider
    {
        private readonly List<IExchangeRateProvider> _localBanks;

        public PolandBanksFacade()
        {
            _localBanks = new List<IExchangeRateProvider>
            {
                new NbpProvider() // National Bank of Poland (Returns rates in PLN base)
            };
        }

        public async Task<List<Currency>> GetRatesAsync()
        {
            // Fetch data from all Polish providers concurrently
            var tasks = _localBanks.Select(async bank =>
            {
                try
                {
                    return await bank.GetRatesAsync();
                }
                catch (Exception ex)
                {
                    // Fault tolerance
                    Console.WriteLine($"[POLAND FACADE WARN] Provider {bank.GetType().Name} failed: {ex.Message}");
                    return new List<Currency>();
                }
            });

            var results = await Task.WhenAll(tasks);
            var validRates = results.Where(list => list != null).SelectMany(list => list).ToList();

            if (validRates.Count == 0)
            {
                Console.WriteLine("[POLAND FACADE CRITICAL] All local Polish providers are unresponsive.");
                return new List<Currency>();
            }

            // Establish the anchor rate 
            var usdRateObj = validRates.FirstOrDefault(c => c.Code == "USD");
            if (usdRateObj == null) return new List<Currency>(); // Failsafe against corrupted API data

            decimal plnToUsdCrossRate = usdRateObj.Value;

            // Normalize all rates to USD base via N.O.R.M.A.
            var allNormalizedRates = new List<Currency>();

            foreach (var rate in validRates)
            {
                if (rate.Code == "USD") continue; // Skip USD as it serves as our absolute anchor (1.0)

                // Apply inverse mathematics for local-base currency system
                var cleanRate = RateNormalizer.NormalizeLocalBase(rate, plnToUsdCrossRate);
                if (cleanRate != null)
                {
                    allNormalizedRates.Add(cleanRate);
                }
            }

            // Manually construct and inject the local base currency (PLN) relative to USD
            allNormalizedRates.Add(new Currency
            {
                Code = "PLN",
                Name = "Polski Złoty",
                Value = 1m / plnToUsdCrossRate, 
                Source = "Poland Facade"
            });

            // Data Consensus
            var finalConsensusRates = allNormalizedRates
                .GroupBy(c => c.Code)
                .Select(group => new Currency
                {
                    Code = group.Key,
                    Name = group.First().Name,
                    Value = group.Average(c => c.Value),
                    Source = "Poland Facade (Consensus)"
                })
                .ToList();

            return finalConsensusRates;
        }
    }
}