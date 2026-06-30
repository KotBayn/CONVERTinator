using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CONVERTinator.Domain.Entities;
using CONVERTinator.Domain.Interfaces;
using CONVERTinator.Helpers;
using CONVERTinator.Services.Regions.CIS.Providers.Russia;


namespace CONVERTinator.Services.Regions.CIS.Facades
{
    public class RussiaBanksFacade : IExchangeRateProvider
    {
        private readonly List<IExchangeRateProvider> _localBanks;

        public RussiaBanksFacade()
        {
            _localBanks = new List<IExchangeRateProvider>
            {
                new CbrProvider()
            };
        }

        public async Task<List<Currency>> GetRatesAsync()
        {
            var tasks = _localBanks.Select(async bank =>
            {
                try { return await bank.GetRatesAsync(); }
                catch (Exception ex)
                {
                    Console.WriteLine($"[RUSSIA FACADE WARN] {bank.GetType().Name} died: {ex.Message}");
                    return new List<Currency>();
                }
            });

            var results = await Task.WhenAll(tasks);
            var validRates = results.Where(list => list != null).SelectMany(list => list).ToList();

            if (!validRates.Any()) return new List<Currency>();

            var normalizedRates = new List<Currency>();

            // Search USD in USD 
            var usdRateObj = validRates.FirstOrDefault(c => c.Code == "USD");
            if (usdRateObj == null) return new List<Currency>();    // If, the bank did not provide the dollar rate - the facade returns empty to avoid poisoning the median
            decimal rubToUsdCrossRate = usdRateObj.Value; 

            //  N.O.R.M.A.
            foreach (var rate in validRates)
            {
                if (rate.Code == "USD") continue;
                var cleanRate = RateNormalizer.NormalizeLocalBase(rate, rubToUsdCrossRate);
                if (cleanRate != null)
                {
                    normalizedRates.Add(cleanRate);
                }
            }

            // Manually add the Russian Ruble to Dollar
            normalizedRates.Add(new Currency
            {
                Code = "RUB",
                Name = "Russian Ruble",
                Value = rubToUsdCrossRate,
                Source = "Russia Facade"
            });

            // If there will be 2-3 banks, we will call grouping and Average() as in Germany.
            // But for now, with only one bank, we simply return the result.
            return normalizedRates;
        }
    }
}