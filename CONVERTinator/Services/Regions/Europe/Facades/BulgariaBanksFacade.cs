using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CONVERTinator.Domain;
using CONVERTinator.Services.Regions.Europe.Providers;

namespace CONVERTinator.Services.Regions.Europe.Facades
{
    public class BulgariaBanksFacade : IExchangeRateProvider
    {
        private readonly BnbProvider _bnbProvider = new BnbProvider();
        private readonly EcbXmlProvider _ecbProvider = new EcbXmlProvider();

        public async Task<List<Currency>> GetRatesAsync()
        {
            var result = new List<Currency>();

            try
            {
                // Get course EUR/BGN from BNB
                var bnbRates = await _bnbProvider.GetRatesAsync();
                decimal eurToBgn = bnbRates.FirstOrDefault(c => c.Code == "EUR")?.Value ?? 1.95583m;

                // Get exchange rates from ECB (all rates to EUR)
                var ecbRates = await _ecbProvider.GetRatesAsync();

                decimal eurToUsd = ecbRates.FirstOrDefault(c => c.Code == "USD")?.Value ?? 1.0m;

                // Calculate the BGN/USD rate
                // If 1 EUR = 1.95583 BGN and 1 EUR = 1.09 USD
                // Then 1 BGN = 1.09 / 1.95583 = 0.557 USD
                decimal bgnToUsd = eurToUsd / eurToBgn;

                result.Add(new Currency
                {
                    Code = "BGN",
                    Name = "Bulgarian lev",
                    Value = bgnToUsd,
                    Source = "BNB (pegged to EUR) + ECB"
                });

                // Convert all other rates from EUR to BGN
                // ECB provides "1 EUR = X foreign currency"
                // "1 BGN = X foreign currency"
                foreach (var currency in ecbRates)
                {
                    if (currency.Code == "EUR") continue; 
                    decimal rateInBgn = currency.Value / eurToBgn;

                    result.Add(new Currency
                    {
                        Code = currency.Code,
                        Name = currency.Name,
                        Value = rateInBgn,
                        Source = $"ECB (via BNB peg)"
                    });
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[BulgariaBanksFacade Error]: {ex.Message}");
            }

            return result;
        }
    }

    public class BnbProvider : IExchangeRateProvider
    {
        public Task<List<Currency>> GetRatesAsync()
        {
            // Fixed rate since 1999, does not change
            return Task.FromResult(new List<Currency>
            {
                new Currency
                {
                    Code = "EUR",
                    Name = "Euro",
                    Value = 1.95583m, // 1 EUR = 1.95583 BGN
                    Source = "Bulgarian National Bank (pegged)"
                }
            });
        }
    }
}