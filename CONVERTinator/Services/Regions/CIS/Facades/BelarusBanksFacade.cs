using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CONVERTinator.Domain;
using CONVERTinator.Helpers;
using CONVERTinator.Services.Regions.CIS.Providers.Belarus;

namespace CONVERTinator.Services.Regions.CIS.Facades
{
    public class BelarusBanksFacade : IExchangeRateProvider

    {
        private readonly List<IExchangeRateProvider> _localBanks;

        public BelarusBanksFacade()
        {
            _localBanks = new List<IExchangeRateProvider>
            {
                new NbrbProvider() // National Bank of Belarus (Base: BYN)
            };
        }
        public async Task<List<Currency>> GetRatesAsync()
        {
            var tasks = _localBanks.Select(async bank =>
            {
                try {return await bank.GetRatesAsync(); }
                catch (Exception ex)
                {
                    Console.WriteLine($"[BELARUS FACADE WARN] {bank.GetType().Name} died: {ex.Message}");
                    return new List<Currency>();
                }
            });

            var results = await Task.WhenAll(tasks);
            var validRates = results.Where(list => list != null).SelectMany(list => list).ToList();

            if (!validRates.Any()) return new List<Currency>();

            var normalizedRates = new List<Currency>();
            var usdRateObj = validRates.FirstOrDefault(c => c.Code == "USD");
            if (usdRateObj == null) return new List<Currency>();
            decimal brubToUsdCrossRate = usdRateObj.Value;

            //  N.O.R.M.A.
            foreach (var rate in validRates)
            {
                if (rate.Code == "USD") continue;
                var cleanRate = RateNormalizer.NormalizeLocalBase(rate, brubToUsdCrossRate);
                if (cleanRate != null)
                {
                    normalizedRates.Add(cleanRate);
                }
            }

            // Manually add the Belarusian Ruble to Dollar
            normalizedRates.Add(new Currency
            {
                Code = "BYN",
                Name = "Belarusian Ruble",
                Value = brubToUsdCrossRate,
                Source = "Belarus Facade"
            });
            return normalizedRates;
        }
    }
}
