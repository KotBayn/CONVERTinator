using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using CONVERTinator.Domain.Entities;
using CONVERTinator.Domain.Interfaces;

namespace CONVERTinator.Services.Providers
{
    public class RegionalFloatRatesProvider : IExchangeRateProvider
    {
        private readonly HttpClient _httpClient;
        private readonly string _baseCurrencyCode;
        private readonly string _url;
        private readonly string _sourceName;

        // FAKER: DO NOT TOUCH!!
        public RegionalFloatRatesProvider(string baseCurrencyCode, string sourceName)
            : this(new HttpClient(), baseCurrencyCode, sourceName)
        {
        }

        // Constructor injection allows us to use this single class for Japan, Australia, India, etc.
        public RegionalFloatRatesProvider(HttpClient httpClient, string baseCurrencyCode, string sourceName)
        {
            _httpClient = httpClient;
            _baseCurrencyCode = baseCurrencyCode.ToLower();
            _url = $"http://www.floatrates.com/daily/{_baseCurrencyCode}.json";
            _sourceName = sourceName;
        }

        public async Task<List<Currency>> GetRatesAsync()
        {
            var result = new List<Currency>();
            try
            {
                string json = await _httpClient.GetStringAsync(_url);
                using JsonDocument doc = JsonDocument.Parse(json);

                foreach (JsonProperty property in doc.RootElement.EnumerateObject())
                {
                    JsonElement currencyData = property.Value;
                    JsonElement rateElement = currencyData.GetProperty("rate");

                    decimal finalRate = 0m;

                    // Robust parsing algorithm to handle both Number and String JSON types
                    if (rateElement.ValueKind == JsonValueKind.Number)
                    {
                        finalRate = rateElement.GetDecimal();
                    }
                    else if (rateElement.ValueKind == JsonValueKind.String)
                    {
                        string textValue = rateElement.GetString();
                        finalRate = Convert.ToDecimal(textValue, System.Globalization.CultureInfo.InvariantCulture);
                    }

                    result.Add(new Currency
                    {
                        Code = currencyData.GetProperty("code").GetString().ToUpper(),
                        Name = currencyData.GetProperty("name").GetString(),
                        Value = finalRate,
                        Source = _sourceName
                    });
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[{_sourceName} Provider Error]: {ex.Message}");
            }
            return result;
        }
    }
}