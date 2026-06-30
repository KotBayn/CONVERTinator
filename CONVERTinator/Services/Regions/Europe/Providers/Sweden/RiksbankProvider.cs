using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using CONVERTinator.Domain.Entities;
using CONVERTinator.Domain.Interfaces;

namespace CONVERTinator.Services.Regions.Europe.Providers.Sweden
{
    public class RiksbankProvider : IExchangeRateProvider
    {
        private readonly HttpClient _httpClient = new HttpClient();
        // API Riksbank
        private const string Url = "https://api.riksbank.se/v1/exchange-rates";
        public async Task<List<Currency>> GetRatesAsync()
        {
            var result = new List<Currency>();
            try
            {
                string json = await _httpClient.GetStringAsync(Url);
                using JsonDocument doc = JsonDocument.Parse(json);

                foreach (JsonElement element in doc.RootElement.EnumerateArray())
                {
                    if (element.GetProperty("base_ccy").GetString() != "SEK") continue;

                    // Parse strings to decimals (Riksbank likes to send "10.50000")
                    decimal buy = Convert.ToDecimal(element.GetProperty("buy").GetString(), System.Globalization.CultureInfo.InvariantCulture);
                    decimal sale = Convert.ToDecimal(element.GetProperty("sale").GetString(), System.Globalization.CultureInfo.InvariantCulture);

                    result.Add(new Currency
                    {
                        Code = element.GetProperty("ccy").GetString(),
                        Name = element.GetProperty("ccy").GetString() + " (Riksbank)",
                        Value = (buy + sale) / 2m,
                        Source = "Riksbank"
                    });
                }
            }
            catch (Exception ex) { Console.WriteLine($"[Riksbank Error]: {ex.Message}"); }
            return result;
        }
    }
}