using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using CONVERTinator.Domain;

namespace CONVERTinator.Services.Regions.Europe.Providers.Ukraine
{
    public class PrivatBankProvider : IExchangeRateProvider
    {
        private readonly HttpClient _httpClient = new HttpClient();
        // API PrivatBank
        private const string Url = "https://api.privatbank.ua/p24api/pubinfo?exchange&coursid=5";

        public async Task<List<Currency>> GetRatesAsync()
        {
            var result = new List<Currency>();
            try
            {
                string json = await _httpClient.GetStringAsync(Url);
                using JsonDocument doc = JsonDocument.Parse(json);

                foreach (JsonElement element in doc.RootElement.EnumerateArray())
                {
                    if (element.GetProperty("base_ccy").GetString() != "UAH") continue;

                    // Parse strings to decimals (Privat likes to send "40.50000")
                    decimal buy = Convert.ToDecimal(element.GetProperty("buy").GetString(), System.Globalization.CultureInfo.InvariantCulture);
                    decimal sale = Convert.ToDecimal(element.GetProperty("sale").GetString(), System.Globalization.CultureInfo.InvariantCulture);

                    result.Add(new Currency
                    {
                        Code = element.GetProperty("ccy").GetString(),
                        Name = element.GetProperty("ccy").GetString() + " (Privat)",
                        Value = (buy + sale) / 2m,
                        Source = "PrivatBank"
                    });
                }
            }
            catch (Exception ex) { Console.WriteLine($"[PrivatBank Error]: {ex.Message}"); }
            return result;
        }
    }
}