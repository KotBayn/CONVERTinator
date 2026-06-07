using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using CONVERTinator.Domain;

namespace CONVERTinator.Services.Regions.Europe.Providers
{
    public class BundesbankProvider : IExchangeRateProvider
    {
        private readonly HttpClient _httpClient = new HttpClient();
        // Free API, getting rates with base USD
        private const string Url = "https://api.statdata.bundesbank.de/Rest/data/BBEX3/D.USD";

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
                        Name = property.Name, // This API does not provide full names, only codes
                        Value = property.Value.GetDecimal(),
                        Source = "Bundesbank (Germany)"
                    });
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in Bundesbank provider: {ex.Message}");
            }
            return result;
        }
    }
}
