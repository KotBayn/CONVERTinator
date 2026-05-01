using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using CONVERTinator.Domain;

namespace CONVERTinator.Services
{
    public class ChinaProvider : IExchangeRateProvider
    {
        private readonly HttpClient _httpClient = new HttpClient();
        // Using a backup API with base in Chinese Yuan (CNY)
        private const string Url = "https://api.exchangerate-api.com/v4/latest/CNY";

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
                        Source = "China Exchange"
                    });
                }
            }
            catch (Exception ex) { Console.WriteLine($"Error in China Provider: {ex.Message}"); }
            return result;
        }
    }
}