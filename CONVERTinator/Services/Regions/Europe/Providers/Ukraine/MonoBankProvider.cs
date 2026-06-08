using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using CONVERTinator.Domain;

namespace CONVERTinator.Services.Regions.Europe.Providers.Ukraine
{
    public class MonoBankProvider : IExchangeRateProvider
    {
        private readonly HttpClient _httpClient = new HttpClient();
        private const string Url = "https://api.monobank.ua/bank/currency";

        public async Task<List<Currency>> GetRatesAsync()
        {
            var result = new List<Currency>();
            try
            {
                string json = await _httpClient.GetStringAsync(Url);
                using JsonDocument doc = JsonDocument.Parse(json);

                foreach (JsonElement element in doc.RootElement.EnumerateArray())
                {
                    // Code 980 - (UAH)
                    if (element.GetProperty("currencyCodeB").GetInt32() != 980) continue;

                    int codeA = element.GetProperty("currencyCodeA").GetInt32();
                    string currencyCode = ResolveIsoCode(codeA);

                    if (string.IsNullOrEmpty(currencyCode)) continue;

                    decimal buy = element.GetProperty("rateBuy").GetDecimal();
                    decimal sell = element.GetProperty("rateSell").GetDecimal();

                    result.Add(new Currency
                    {
                        Code = currencyCode,
                        Name = currencyCode + " (Mono)",
                        Value = (buy + sell) / 2m,
                        Source = "Monobank"
                    });
                }
            }
            catch (Exception ex) { Console.WriteLine($"[Monobank Error]: {ex.Message}"); }
            return result;
        }

        // Dictionary of numeric codes to ISO codes
        private string ResolveIsoCode(int isoNumericCode)
        {
            return isoNumericCode switch
            {
                840 => "USD",
                978 => "EUR",
                826 => "GBP",
                985 => "PLN",
                203 => "CZK",
                _ => null // Остальные нам не нужны, НБУ их и так даст
            };
        }
    }
}