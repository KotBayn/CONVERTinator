using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using CONVERTinator.Domain;

namespace CONVERTinator.Services.Regions.Europe.Providers.Czech
{
    public class CnbProvider : IExchangeRateProvider
    {
        private readonly HttpClient _httpClient = new HttpClient();
        // Official CNB daily fixing text file
        private const string Url = "https://www.cnb.cz/en/financial-markets/foreign-exchange-market/central-bank-exchange-rate-fixing/central-bank-exchange-rate-fixing/daily.txt";

        public async Task<List<Currency>> GetRatesAsync()
        {
            var result = new List<Currency>();
            try
            {
                string text = await _httpClient.GetStringAsync(Url);

                // Split the text into lines, handling both Windows and Linux line endings
                string[] lines = text.Split(new[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);

                // The first two lines are Date and Header. Start parsing from line 2 (index 2).
                // Example format: Country|Currency|Amount|Code|Rate
                //                 USA|dollar|1|USD|22.500
                for (int i = 2; i < lines.Length; i++)
                {
                    string[] parts = lines[i].Split('|');
                    if (parts.Length < 5) continue; // Safety check

                    string country = parts[0];
                    string currencyName = parts[1];
                    decimal amount = Convert.ToDecimal(parts[2], System.Globalization.CultureInfo.InvariantCulture);
                    string code = parts[3];
                    decimal rate = Convert.ToDecimal(parts[4], System.Globalization.CultureInfo.InvariantCulture);

                    result.Add(new Currency
                    {
                        Code = code,
                        Name = $"{country} {currencyName}",
                        // Neutralizing the nominal trap (e.g. 100 HUF = 6.45 CZK)
                        Value = rate / amount,
                        Source = "CNB (Czechia)"
                    });
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[CNB Provider Error]: {ex.Message}");
            }
            return result;
        }
    }
}