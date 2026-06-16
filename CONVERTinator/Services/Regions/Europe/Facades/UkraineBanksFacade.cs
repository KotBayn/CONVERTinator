using CONVERTinator.Domain;
using CONVERTinator.Helpers;
using CONVERTinator.Services.Regions.Europe.Providers.Ukraine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CONVERTinator.Services.Regions.Europe.Facades
{
    internal class UkraineBanksFacade : IExchangeRateProvider
    {
        private readonly List<IExchangeRateProvider> _localBanks;

        public UkraineBanksFacade()
        {
            _localBanks = new List<IExchangeRateProvider>
            {
                new NbuProvider(), // National Bank of Ukraine (Returns rates in UAH base)
                new PrivatBankProvider(), // PrivatBank (Returns rates in UAH base)
                new MonoBankProvider() // Monobank (Returns rates in UAH base)
            };
        }

        public async Task<List<Currency>> GetRatesAsync()
        {
            // Concurrent execution of Ukrainian data providers
            var tasks = _localBanks.Select(async bank =>
            {
                try { return await bank.GetRatesAsync(); }
                catch (Exception ex)
                {
                    Console.WriteLine($"[UKRAINE FACADE WARN] Provider {bank.GetType().Name} failed: {ex.Message}");
                    return new List<Currency>();
                }
            });

            var results = await Task.WhenAll(tasks);
            var validRates = results.Where(list => list != null).SelectMany(list => list).ToList();

            if (validRates.Count == 0)
            {
                Console.WriteLine("[UKRAINE FACADE CRITICAL] Ukrainian banking API is unreachable.");
                return new List<Currency>();
            }

            // Locate the anchor
            var usdRateObj = validRates.FirstOrDefault(c => c.Code == "USD");
            if (usdRateObj == null) return new List<Currency>(); // Prevent calculation if USD anchor is missing

            decimal uahToUsdCrossRate = usdRateObj.Value;

            // Normalize values into USD-centric system via N.O.R.M.A.
            var allNormalizedRates = new List<Currency>();

            foreach (var rate in validRates)
            {
                if (rate.Code == "USD") continue;

                var cleanRate = RateNormalizer.NormalizeLocalBase(rate, uahToUsdCrossRate);
                if (cleanRate != null)
                {
                    allNormalizedRates.Add(cleanRate);
                }
            }

            // Inject Ukrainian Hryvnia asset specifications relative to USD
            allNormalizedRates.Add(new Currency
            {
                Code = "UAH",
                Name = "Ukrainian Hryvnia",
                Value = 1m / uahToUsdCrossRate, // 1 USD divided by UAH rate
                Source = "Ukraine Facade"
            });

            // Consensus assembly
            var finalConsensusRates = allNormalizedRates
                .GroupBy(c => c.Code)
                .Select(group => new Currency
                {
                    Code = group.Key,
                    Name = group.First().Name,
                    Value = group.Average(c => c.Value),
                    Source = "Ukraine Facade (Consensus)"
                })
                .ToList();

            return finalConsensusRates;
        }
    }
}