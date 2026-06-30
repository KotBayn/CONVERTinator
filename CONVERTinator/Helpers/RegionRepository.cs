using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection.Metadata;
using System.Text.Json;
using CONVERTinator.Domain.GEO;
using CONVERTinator.Domain;

namespace CONVERTinator.Helpers
{
    public static class RegionRepository
    {
        // Dictionary to hold parsed JSON data in memory for blazing fast access
        private static readonly Dictionary<Region, List<string>> RegionCurrencies;
        static RegionRepository()
        {
            RegionCurrencies = new Dictionary<Region, List<string>>();
            LoadFromJson();
        }

        private static void LoadFromJson()
        {
            string filePath = "RegionsCurrencies.json";
            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException($"CRITICAL ERROR: Configuration file '{filePath}' is missing! The BUSINESS mode cannot function.");
            }

            try
            {
                // Read, Parse, Iterate the entire JSON file
                string jsonString = File.ReadAllText(filePath);
                using JsonDocument doc = JsonDocument.Parse(jsonString);

                foreach (JsonProperty property in doc.RootElement.EnumerateObject())
                {
                    // Attempt to map the JSON string key to our Region enum
                    if (Enum.TryParse(property.Name, out Region regionEnum))
                    {
                        var currencies = new List<string>();
                        foreach (JsonElement currencyElement in property.Value.EnumerateArray())
                        {
                            currencies.Add(currencyElement.GetString());
                        }

                        // Store in our fast in-memory dictionary
                        RegionCurrencies[regionEnum] = currencies;
                    }
                }
            }
            catch (JsonException ex)
            {
                throw new FormatException($"CRITICAL ERROR: The file '{filePath}' contains invalid JSON format. Details: {ex.Message}");
            }
        }

        // Fetches the list of currencies associated with a specific region
        public static List<string> GetCurrenciesByRegion(Region region)
        {
            if (RegionCurrencies.TryGetValue(region, out var currencies))
            {
                return currencies;
            }

            if (RegionCurrencies.TryGetValue(Region.Global, out var globalCurrencies))
            {
                return globalCurrencies;
            }

            return new List<string> { Constants.MainCurrency.USD, Constants.MainCurrency.EUR };
        }
    }
}