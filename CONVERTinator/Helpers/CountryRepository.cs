using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using CONVERTinator.Domain;
using CONVERTinator.Domain.GEO;


namespace CONVERTinator.Helpers
{
    public static class CountryRepository
    {
        // Local country database (Graph). 
        // Connections are built by ISO codes (including maritime borders for islands).
        private static readonly Dictionary<string, Country> Countries;
        
        static CountryRepository()
        {
            string baseDir = AppContext.BaseDirectory;
            Console.WriteLine("\n===== DEBUG INFO =====");
            Console.WriteLine($"[DEBUG] Base Directory: {baseDir}");

            string filePath = Path.Combine(baseDir, "Services", "Countries.json");
            Console.WriteLine($"[DEBUG] Expected file path: {filePath}");

            bool exists = File.Exists(filePath);
            Console.WriteLine($"[DEBUG] File exists: {exists}");
            Console.WriteLine("======================\n");
            // Load the country graph from a JSON file at startup
            if (exists)
            {
                try
                {
                    string json = File.ReadAllText(filePath);
                    var options = new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true,
                        Converters = { new JsonStringEnumConverter() }
                    };
                    Countries = JsonSerializer.Deserialize<Dictionary<string, Country>>(json, options) 
                                ?? throw new InvalidOperationException("Failed to deserialize country data.");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[CRITICAL] Failed to parse: {ex.Message}");
                    Countries = new Dictionary<string, Country>();
                }
            }
            else
            {
                Console.WriteLine($"[CRITICAL] countries.json file not found at path: {filePath}");
                Countries = new Dictionary<string, Country>();
            }
        }
        public static Country GetCountryByIso(string isoCode)
        {
            if (Countries.TryGetValue(isoCode.ToUpper(), out var country))
            {
                return country;
            }

            // Fallback: If country is missing in our graph, return a global stub
            return new Country
            {
                IsoCode = isoCode.ToUpper(),
                CurrencyCode = Constants.MainCurrency.USD,
                CountryRegion = Region.Global,
                Neighbors = new List<string>()
            };
        }

        public static List<string> GetTravelCurrencies(string currentIsoCode)
        {
            var country = GetCountryByIso(currentIsoCode);
            var currencies = new HashSet<string> { country.CurrencyCode };

            foreach (var neighborIso in country.Neighbors)
            {
                var neighbor = GetCountryByIso(neighborIso);
                currencies.Add(neighbor.CurrencyCode);
            }

            // FOOLPROOF: If we have less than 3 unique currencies, inject major global ones
            if (currencies.Count < 3) currencies.Add(Constants.MainCurrency.USD);
            if (currencies.Count < 3) currencies.Add(Constants.MainCurrency.EUR);
            if (currencies.Count < 3) currencies.Add("GBP");

            // Always enforce global anchor currencies availability
            currencies.Add(Constants.MainCurrency.USD);
            currencies.Add(Constants.MainCurrency.EUR);

            return currencies.ToList();
        }
    
        /// <summary>
        /// Dynamic Zone Resolver: Calculates which financial clusters (Regions) 
        /// must be loaded based on the host country and its immediate geopolitical neighbors.
        /// </summary>
        public static HashSet<Region> GetRequiredRegions(string currentIsoCode)
        {
            var activeRegions = new HashSet<Region>();

            // Identify the host country and load its primary region
            var hostCountry = GetCountryByIso(currentIsoCode);
            activeRegions.Add(hostCountry.CountryRegion);

            // Scan all bordering countries to dynamically expand the data cluster
            foreach (var neighborIso in hostCountry.Neighbors)
            {
                var neighbor = GetCountryByIso(neighborIso);
                activeRegions.Add(neighbor.CountryRegion);
            }

            // Always ensure the Americas are loaded if you want the global USD anchor to be rock-solid.
            // If the user travels to a deep isolated region without USD, the facades still calculate it.

            return activeRegions;
        } 
    }
}