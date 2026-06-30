using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using CONVERTinator.Domain.Entities;
using CONVERTinator.Domain.Interfaces;

namespace CONVERTinator.Services.Regions.Europe.Providers.Poland
{
    public class NbpProvider : IExchangeRateProvider
    {
        private readonly HttpClient _httpClient = new HttpClient();
        private const string Url = "http://api.nbp.pl/api/exchangerates/tables/a/?format=json";

        public async Task<List<Currency>> GetRatesAsync()
        {
            var result = new List<Currency>();
            try
            {
                string json = await _httpClient.GetStringAsync(Url);
                using JsonDocument doc = JsonDocument.Parse(json);
                JsonElement rootArray = doc.RootElement[0];
                JsonElement ratesArray = rootArray.GetProperty("rates");

                foreach (JsonElement rate in ratesArray.EnumerateArray())
                {
                    result.Add(new Currency
                    {
                        Code = rate.GetProperty("code").GetString(),
                        Name = rate.GetProperty("currency").GetString(), // Names may be in Polish (dolar amerykański)
                        // IMPORTANT: API provides the price of 1 foreign currency in Polish zloty.
                        Value = rate.GetProperty("mid").GetDecimal(),
                        Source = "NBP (Poland)"
                    });
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[NBP Provider Error]: {ex.Message}");
            }
            return result;
        }
    }
}