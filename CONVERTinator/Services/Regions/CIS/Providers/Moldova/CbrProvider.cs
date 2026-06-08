using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using CONVERTinator.Domain;

namespace CONVERTinator.Services.Regions.CIS.Providers
{
    public class CbrProvider : IExchangeRateProvider
    {
        private readonly HttpClient _httpClient = new HttpClient();
        private const string Url = "https://www.cbr-xml-daily.ru/daily_json.js";

        public async Task<List<Currency>> GetRatesAsync()
        {
            var result = new List<Currency>();

            try
            {
                // 1. download the raw JSON string
                string json = await _httpClient.GetStringAsync(Url);

                // 2. convert the text into a convenient document
                using JsonDocument doc = JsonDocument.Parse(json);
                JsonElement root = doc.RootElement;
                JsonElement valuteElement = root.GetProperty("Valute");

                // 3. iterate over each currency provided by the bank
                foreach (JsonProperty property in valuteElement.EnumerateObject())
                {
                    JsonElement currencyData = property.Value;
                    decimal nominal = currencyData.GetProperty("Nominal").GetDecimal();
                    decimal rawValue = currencyData.GetProperty("Value").GetDecimal();

                    // 4. filling in clean template
                    var currency = new Currency
                    {
                        Code = currencyData.GetProperty("CharCode").GetString(),
                        Name = currencyData.GetProperty("Name").GetString(),

                        Value = rawValue / nominal,
                        Source = "CBR"
                    };

                    result.Add(currency);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in CBR journalist: {ex.Message}");
            }

            return result;
        }
    }
}