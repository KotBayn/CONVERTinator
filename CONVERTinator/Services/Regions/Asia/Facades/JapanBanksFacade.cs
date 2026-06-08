using CONVERTinator.Domain;
using CONVERTinator.Helpers;
using CONVERTinator.Services.Providers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CONVERTinator.Services.Regions.Asia.Facades
{
    internal class JapanBanksFacade : IExchangeRateProvider
    {
        private readonly List<IExchangeRateProvider> _localBanks;

        public JapanBanksFacade()
        {
            _localBanks = new List<IExchangeRateProvider>
            {
                // Using universal provider configured specifically for Japan
                new RegionalFloatRatesProvider("jpy", "Bank of Japan (via FloatRates)")
            };
        }

        public async Task<List<Currency>> GetRatesAsync()
        {
            var tasks = _localBanks.Select(async bank =>
            {
                try { return await bank.GetRatesAsync(); }
                catch (Exception ex)
                {
                    Console.WriteLine($"[JAPAN FACADE WARN] Provider failed: {ex.Message}");
                    return new List<Currency>();
                }
            });

            var results = await Task.WhenAll(tasks);
            var validRates = results.Where(list => list != null).SelectMany(list => list).ToList();

            if (validRates.Count == 0) return new List<Currency>();

            // Anchor: How many JPY for 1 USD
            var usdRateObj = validRates.FirstOrDefault(c => c.Code == "USD");
            if (usdRateObj == null) return new List<Currency>();

            decimal jpyToUsdCrossRate = usdRateObj.Value;

            var allNormalizedRates = new List<Currency>();

            // N.O.R.M.A.
            foreach (var rate in validRates)
            {
                if (rate.Code == "USD") continue;

                var cleanRate = RateNormalizer.NormalizeForeignBase(rate, jpyToUsdCrossRate);
                if (cleanRate != null) allNormalizedRates.Add(cleanRate);
            }

            // Inject Japanese Yen (JPY) explicitly
            allNormalizedRates.Add(new Currency
            {
                Code = "JPY",
                Name = "Japanese Yen",
                Value = 1m / jpyToUsdCrossRate,
                Source = "Japan Facade"
            });

            return allNormalizedRates;
        }
    }
}