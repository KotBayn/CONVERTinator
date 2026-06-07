using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CONVERTinator.Domain;

namespace CONVERTinator.Services.Regions.Europe.Providers
{
    public class BnbProvider : IExchangeRateProvider
    {
        // Bulgarian lev is pegged to the euro since 1999
        // Currency board: 1 EUR = 1.95583 BGN (fixed)
        private const decimal BgnToEurRate = 1.95583m;

        public Task<List<Currency>> GetRatesAsync()
        {
            var result = new List<Currency>
            {
                new Currency
                {
                    Code = "EUR",
                    Name = "Euro",
                    // Course 1 EUR to BGN
                    Value = BgnToEurRate,
                    Source = "Bulgarian National Bank (pegged)"
                }
            };

            return Task.FromResult(result);
        }
    }
}