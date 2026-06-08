using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using CONVERTinator.Domain;

namespace CONVERTinator.Services.Regions.AmericasN.Providers
{
    public class UsProvider : IExchangeRateProvider
    {
        private readonly HttpClient _httpClient = new HttpClient();

        // Highly stable free tier API. Crucially, its base is strictly USD.
        // This acts as our absolute mathematical anchor for the American region.
        private const string Url = "https://open.er-api.com/v6/latest/USD";

        public async Task<List<Currency>> GetRatesAsync()
        {
            var result = new List<Currency>();
            try
            {
                string json = await _httpClient.GetStringAsync(Url);
                using JsonDocument doc = JsonDocument.Parse(json);
                JsonElement ratesElement = doc.RootElement.GetProperty("rates");

                foreach (JsonProperty property in ratesElement.EnumerateObject())
                {
                    result.Add(new Currency
                    {
                        Code = property.Name,
                        Name = property.Name,
                        Value = property.Value.GetDecimal(),
                        Source = "US Global API"
                    });
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[US Provider Error]: {ex.Message}");
            }
            return result;
        }
    }
}