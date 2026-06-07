using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using CONVERTinator.Domain;

namespace CONVERTinator.Services.Regions.Europe.Providers
{
    public class FloatRatesProvider : IExchangeRateProvider
    {
        private readonly HttpClient _httpClient = new HttpClient();
        private const string Url = "http://www.floatrates.com/daily/usd.json";

        public async Task<List<Currency>> GetRatesAsync()
        {
            var result = new List<Currency>();
            try
            {
                string json = await _httpClient.GetStringAsync(Url);
                using JsonDocument doc = JsonDocument.Parse(json);

                // FloatRates root IS the dictionary of currencies itself.
                foreach (JsonProperty property in doc.RootElement.EnumerateObject())
                {
                    JsonElement currencyData = property.Value;

                    result.Add(new Currency
                    {
                        Code = currencyData.GetProperty("code").GetString().ToUpper(),
                        Name = currencyData.GetProperty("name").GetString(),
                        // FloatRates returns "rate" as: How many units of X for 1 USD. 
                        Value = currencyData.GetProperty("rate").GetDecimal(),
                        Source = "FloatRates Aggregator"
                    });
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[FloatRates Journalistic Error]: {ex.Message}");
            }
            return result;
        }
    }
}