using CONVERTinator.Domain;
using CONVERTinator.Helpers;
using CONVERTinator.Services.Regions.Europe.Providers.Germany;
using CONVERTinator.Services.Regions.Europe.Providers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CONVERTinator.Services.Regions.Europe.Facades
{
    internal class GermanyBanksFacade : IExchangeRateProvider
    {
        private readonly List<IExchangeRateProvider> _localBanks;

        public GermanyBanksFacade()
        {
            _localBanks = new List<IExchangeRateProvider>
            {
                new EcbProvider(),   // Returns rates already anchored to USD Base
                new EcbXmlProvider(), // Returns rates anchored to EUR Base (requires conversion)
                new FloatRatesProvider(), // Returns rates anchored to USD Base
                new FloatRatesXmlProvider(), // Returns rates anchored to EUR Base (requires conversion)
                new BundesbankProvider(), // Returns rates anchored to EUR Base (requires conversion)
                new BundesbankXmlProvider() // Returns rates anchored to EUR Base (requires conversion)
            };
        }

        public async Task<List<Currency>> GetRatesAsync()
        {
            // Fetch data from all German/EU providers asynchronously
            var tasks = _localBanks.Select(async bank =>
            {
                try { return await bank.GetRatesAsync(); }
                catch (Exception ex)
                {
                    // Fault tolerance
                    Console.WriteLine($"[GERMANY FACADE WARN] Provider {bank.GetType().Name} failed: {ex.Message}");
                    return new List<Currency>();
                }
            });

            var results = await Task.WhenAll(tasks);
            var validRates = results.Where(list => list != null).SelectMany(list => list).ToList();

            if (validRates.Count == 0)
            {
                Console.WriteLine("[GERMANY FACADE CRITICAL] All European infrastructure providers are down.");
                return new List<Currency>();
            }

            // Extract the EUR/USD cross-rate from the XML provider to use as an anchor
            var usdFromXml = validRates.FirstOrDefault(c => c.Code == "USD" && c.Source == "ECB (XML)");
            decimal eurToUsdExchangeRate = usdFromXml?.Value ?? 1.08m; // Fallback to standard parity if XML fails

            // Process normalization based on data origin
            var normalizedRates = new List<Currency>();

            foreach (var rate in validRates)
            {
                if (rate.Code == "USD") continue;

                if (rate.Source == "ECB (XML)")
                {
                    // XML data is EUR-based -> Use Foreign Base normalization
                    var cleanRate = RateNormalizer.NormalizeForeignBase(rate, eurToUsdExchangeRate);
                    if (cleanRate != null) normalizedRates.Add(cleanRate);
                }
                else
                {
                    // Frankfurter JSON is already USD-based -> Keep as is
                    normalizedRates.Add(rate);
                }
            }

            // Manually inject Euro currency token into the matrix
            if (!normalizedRates.Any(c => c.Code == "EUR"))
            {
                normalizedRates.Add(new Currency
                {
                    Code = "EUR",
                    Name = "Euro",
                    Value = 1m / eurToUsdExchangeRate,
                    Source = "Germany Facade"
                });
            }

            // Data Consensus: Group by currency code and calculate mathematical average
            var finalConsensusRates = normalizedRates
                .GroupBy(c => c.Code)
                .Select(group => new Currency
                {
                    Code = group.Key,
                    Name = group.First().Name,
                    Value = group.Average(c => c.Value),
                    Source = "Germany Facade (Consensus)"
                })
                .ToList();

            return finalConsensusRates;
        }
    }
}