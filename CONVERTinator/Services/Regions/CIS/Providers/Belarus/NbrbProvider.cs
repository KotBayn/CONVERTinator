using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using CONVERTinator.Domain.Entities;
using CONVERTinator.Domain.Interfaces;

namespace CONVERTinator.Services.Regions.CIS.Providers.Belarus
{
    public class NbrbProvider : IExchangeRateProvider
    {
        private readonly HttpClient _httpClient = new HttpClient();
        // Official JSON API of the National Bank of Belarus. Base - BYN.
        private const string Url = "https://api.nbrb.by/exrates/rates?periodicity=0";

        public async Task<List<Currency>> GetRatesAsync()
        {
            var result = new List<Currency>();
            try
            {
                string json = await _httpClient.GetStringAsync(Url);
                using JsonDocument doc = JsonDocument.Parse(json);
                foreach (JsonElement element in doc.RootElement.EnumerateArray())
                {
                    decimal scale = element.GetProperty("Cur_Scale").GetDecimal();
                    decimal officialRate = element.GetProperty("Cur_OfficialRate").GetDecimal();

                    result.Add(new Currency
                    {
                        Code = element.GetProperty("Cur_Abbreviation").GetString(),
                        Name = element.GetProperty("Cur_Name").GetString(),
                        Value = officialRate / scale,
                        Source = "National Bank of Belarus"
                    });
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[NBRB Error]: {ex.Message}");
            }
                return result;
        }
    }
}
