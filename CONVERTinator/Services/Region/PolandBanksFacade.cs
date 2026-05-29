using CONVERTinator.Domain;
using CONVERTinator.Services.Providers;
using CONVERTinator.Services.RegionProviders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CONVERTinator.Services.Region
{
    internal class PolandBanksFacade : IExchangeRateProvider 
    {
            private readonly List<IExchangeRateProvider> _localBanks;

            public PolandBanksFacade()
            {
                // First one is JSON, second one is XML.
                _localBanks = new List<IExchangeRateProvider>
            {
                new NbpProvider(),    // Give in PLN (requires conversion)
                //new NbpXmlProvider()  // Give in EUR (requires conversion)
            };
            }

            public async Task<List<Currency>> GetRatesAsync()
            {
                // Parallel call to all local providers
                var tasks = _localBanks.Select(async bank =>
                {
                    try
                    {
                        return await bank.GetRatesAsync();
                    }
                    catch (Exception ex)
                    {
                        // CASCADE FAILURE PROTECTION: if a bank fails, return empty list instead of throwing Exception
                        Console.WriteLine($"[GERMANY FACADE WARN] Provider {bank.GetType().Name} failed: {ex.Message}");
                        return new List<Currency>();
                    }
                });

                // If one hangs, the others will still return data.
                var results = await Task.WhenAll(tasks);
                var validRateLists = results.Where(list => list != null && list.Count > 0).ToList();

                // If all German banks fail
                if (validRateLists.Count == 0)
                {
                    Console.WriteLine("[GERMANY FACADE CRITICAL] All local providers are dead.");
                    return new List<Currency>();
                }

                // Normalize all rates to USD and merge into a single clean list
                var allNormalizedRates = new List<Currency>();

                foreach (var rateList in validRateLists)
                {
                    allNormalizedRates.AddRange(NormalizeToUsd(rateList));
                }

                // CONSENSUS DATA
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

            // Internal method to "clean" the data
            private List<Currency> NormalizeToUsd(List<Currency> rawRates)
            {
                var normalized = new List<Currency>();
                decimal eurToUsdRate = 1m;
                var usdFromXml = rawRates.FirstOrDefault(c => c.Code == "USD" && c.Source == "NBP (XML)");
                if (usdFromXml != null)
                {
                    eurToUsdRate = usdFromXml.Value;
                }

                foreach (var rate in rawRates)
                {
                    if (rate.Code == "USD") continue;

                    // if XML - divide by EUR->USD to get USD value
                    if (rate.Source == "NBP (XML)")
                    {
                        rate.Value = rate.Value / eurToUsdRate;
                    }
                    // if JSON (Frankfurter), they are ALREADY in dollars, do nothing!

                    normalized.Add(rate);
                }

                // Manually add Euro (calculate from found cross-rate)
                if (!normalized.Any(c => c.Code == "EUR"))
                {
                    normalized.Add(new Currency
                    {
                        Code = "EUR",
                        Name = "Euro",
                        Value = 1m / eurToUsdRate,
                        Source = "Poland Facade"
                    });
                }

                return normalized;
            }
    }
}