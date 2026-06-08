using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using CONVERTinator.Domain;

namespace CONVERTinator.Services.Regions.Europe.Providers.Ukraine
{
    public class NbuProvider : IExchangeRateProvider
    {
        private readonly HttpClient _httpClient = new HttpClient();
        // Official API NBU. Returns JSON. Base - UAH.
        private const string Url = "https://bank.gov.ua/NBUStatService/v1/statdirectory/exchange?json";

        public async Task<List<Currency>> GetRatesAsync()
        {
            var result = new List<Currency>();
            try
            {
                string json = await _httpClient.GetStringAsync(Url);
                using JsonDocument doc = JsonDocument.Parse(json);
                foreach (JsonElement element in doc.RootElement.EnumerateArray())
                {
                    result.Add(new Currency
                    {
                        // "cc" - currency code (USD, EUR)
                        Code = element.GetProperty("cc").GetString(),
                        // "txt" - currency name (Dollar USA)
                        Name = element.GetProperty("txt").GetString(),
                        // "rate" - how many hryvnias one unit of this currency costs
                        Value = element.GetProperty("rate").GetDecimal(),
                        Source = "NBU (Ukraine)"
                    });
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[NBU Provider Error]: {ex.Message}");
            }
            return result;
        }
    }
}