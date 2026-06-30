using CONVERTinator.Domain.Entities;
using CONVERTinator.Domain.Interfaces;
using CONVERTinator.Services.Regions.Europe.Providers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CONVERTinator.Services.Regions.Europe.Facades
{
    internal class SpainBanksFacade : IExchangeRateProvider
    {
        private readonly List<IExchangeRateProvider> _localBanks;

        public SpainBanksFacade()
        {
            // Banca d'Italia is part of the Eurosystem. 
            // Use the ECB (Frankfurter API) which already provides USD-anchored rates.
            _localBanks = new List<IExchangeRateProvider>
            {
                new EcbProvider(),
                new EcbXmlProvider() // Backup source for cross-checking EUR/USD rate if needed
            };
        }

        public async Task<List<Currency>> GetRatesAsync()
        {
            try
            {
                // Frankfurter API already normalizes to USD base, no N.O.R.M.A. needed here!
                var rates = await _localBanks[0].GetRatesAsync();

                if (rates == null || !rates.Any()) return new List<Currency>();

                if (!rates.Any(c => c.Code == "EUR"))
                {
                    // Find USD rate to EUR if we need a cross check, 
                    // but EcbProvider gives us 1 EUR = X USD directly.
                    // Actually, if base is USD, the EUR value is exactly "How many EUR for 1 USD".
                    var eurRate = rates.FirstOrDefault(c => c.Code == "EUR");
                    if (eurRate == null)
                    {
                        rates.Add(new Currency { Code = "EUR", Name = "Euro", Value = 0.92m, Source = "Italy Facade (Fallback)" });
                    }
                }

                return rates;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[SPAIN FACADE CRITICAL] Eurosystem unavailable: {ex.Message}");
                return new List<Currency>();
            }
        }
    }
}