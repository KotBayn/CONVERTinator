using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using CONVERTinator.Domain;

namespace CONVERTinator.Services
{
    public class CryptoProvider : IExchangeRateProvider
    {
        private readonly HttpClient _httpClient = new HttpClient();
        // Check only top-15 cryptocurrencies
        private const string Url = "https://api.coincap.io/v2/assets?limit=15";

        public async Task<List<Currency>> GetRatesAsync()
        {
            var result = new List<Currency>();
            try
            {
                string json = await _httpClient.GetStringAsync(Url);
                using JsonDocument doc = JsonDocument.Parse(json);
                JsonElement dataArray = doc.RootElement.GetProperty("data");

                foreach (JsonElement crypto in dataArray.EnumerateArray())
                {
                    // API crypto returns the price as a string
                    string priceString = crypto.GetProperty("priceUsd").GetString();
                    decimal price = Convert.ToDecimal(priceString, System.Globalization.CultureInfo.InvariantCulture);

                    result.Add(new Currency
                    {
                        Code = crypto.GetProperty("symbol").GetString(),
                        Name = crypto.GetProperty("name").GetString(),
                        Value = price,
                        Source = "CoinCap (Crypto)"
                    });
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in Crypto provider: {ex.Message}");
            }
            return result;
        }
    }
}