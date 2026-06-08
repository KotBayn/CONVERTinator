using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using CONVERTinator.Domain;

namespace CONVERTinator.Services.Regions.AmericasN.Providers
{
    public class BankOfCanadaProvider : IExchangeRateProvider
    {
        private readonly HttpClient _httpClient = new HttpClient();

        // Official Bank of Canada Valet API. Returns the most recent daily observations.
        // Note: Rates are local-based (e.g., 1 USD = 1.37 CAD)
        private const string Url = "https://www.bankofcanada.ca/valet/observations/group/FX_RATES_DAILY/json?recent=1";

        public async Task<List<Currency>> GetRatesAsync()
        {
            var result = new List<Currency>();
            try
            {
                string json = await _httpClient.GetStringAsync(Url);
                using JsonDocument doc = JsonDocument.Parse(json);

                // The API returns an array of observations.
                JsonElement observations = doc.RootElement.GetProperty("observations")[0];

                foreach (JsonProperty property in observations.EnumerateObject())
                {
                    string key = property.Name;

                    // Filter only the standard exchange rate pairs (e.g., "FXUSDCAD")
                    if (key.StartsWith("FX") && key.EndsWith("CAD"))
                    {
                        // Extract the exact 3-letter foreign currency code
                        string code = key.Substring(2, 3);
                        string valueStr = property.Value.GetProperty("v").GetString();
                        decimal rate = Convert.ToDecimal(valueStr, System.Globalization.CultureInfo.InvariantCulture);

                        result.Add(new Currency
                        {
                            Code = code,
                            Name = $"{code} (BoC)",
                            Value = rate,
                            Source = "Bank of Canada"
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Bank of Canada Error]: {ex.Message}");
            }
            return result;
        }
    }
}