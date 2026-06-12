using System.Collections.Generic;
using System.Linq;
using CONVERTinator.Domain.GEO;

namespace CONVERTinator.Helpers
{
    public static class CountryRepository
    {
        // Local country database (Graph). 
        // Connections are built by ISO codes (including maritime borders for islands).
        private static readonly Dictionary<string, Country> Countries = new Dictionary<string, Country>
        {
            // --- EUROPE ---
            { "PL", new Country { IsoCode = "PL", CurrencyCode = "PLN", CountryRegion = Region.Europe, Neighbors = new List<string> { "DE", "CZ", "SK", "UA", "BY", "LT", "RU" } } },
            { "DE", new Country { IsoCode = "DE", CurrencyCode = "EUR", CountryRegion = Region.Europe, Neighbors = new List<string> { "DK", "PL", "CZ", "AT", "CH", "FR", "LU", "BE", "NL" } } },
            { "UA", new Country { IsoCode = "UA", CurrencyCode = "UAH", CountryRegion = Region.CIS, Neighbors = new List<string> { "PL", "SK", "HU", "RO", "MD", "RU", "BY" } } },
            { "BY", new Country { IsoCode = "BY", CurrencyCode = "BYN", CountryRegion = Region.CIS, Neighbors = new List<string> { "PL", "LT", "LV", "RU", "UA" } } },
            { "RU", new Country { IsoCode = "RU", CurrencyCode = "RUB", CountryRegion = Region.CIS, Neighbors = new List<string> { "NO", "FI", "EE", "LV", "LT", "PL", "BY", "UA", "GE", "AZ", "KZ", "CN", "MN", "KP" } } },
            
            // Island Nations & Maritime Borders
            { "IS", new Country { IsoCode = "IS", CurrencyCode = "ISK", CountryRegion = Region.Europe, Neighbors = new List<string> { "GB", "NO", "DK", "IE" } } },
            { "GB", new Country { IsoCode = "GB", CurrencyCode = "GBP", CountryRegion = Region.Europe, Neighbors = new List<string> { "IE", "FR", "BE", "NL", "IS" } } },
            { "IE", new Country { IsoCode = "IE", CurrencyCode = "EUR", CountryRegion = Region.Europe, Neighbors = new List<string> { "GB", "IS", "FR" } } },
            
            // Western & Southern Europe
            { "FR", new Country { IsoCode = "FR", CurrencyCode = "EUR", CountryRegion = Region.Europe, Neighbors = new List<string> { "BE", "LU", "DE", "CH", "IT", "MC", "AD", "ES", "GB" } } },
            { "IT", new Country { IsoCode = "IT", CurrencyCode = "EUR", CountryRegion = Region.Europe, Neighbors = new List<string> { "FR", "CH", "AT", "SI", "SM", "VA" } } },
            { "ES", new Country { IsoCode = "ES", CurrencyCode = "EUR", CountryRegion = Region.Europe, Neighbors = new List<string> { "PT", "FR", "AD", "MA" } } },
            { "PT", new Country { IsoCode = "PT", CurrencyCode = "EUR", CountryRegion = Region.Europe, Neighbors = new List<string> { "ES" } } },
            { "NL", new Country { IsoCode = "NL", CurrencyCode = "EUR", CountryRegion = Region.Europe, Neighbors = new List<string> { "DE", "BE", "GB" } } },
            { "BE", new Country { IsoCode = "BE", CurrencyCode = "EUR", CountryRegion = Region.Europe, Neighbors = new List<string> { "FR", "LU", "DE", "NL", "GB" } } },
            { "CH", new Country { IsoCode = "CH", CurrencyCode = "CHF", CountryRegion = Region.Europe, Neighbors = new List<string> { "FR", "DE", "AT", "LI", "IT" } } },
            { "AT", new Country { IsoCode = "AT", CurrencyCode = "EUR", CountryRegion = Region.Europe, Neighbors = new List<string> { "DE", "CZ", "SK", "HU", "SI", "IT", "CH", "LI" } } },
            { "CZ", new Country { IsoCode = "CZ", CurrencyCode = "CZK", CountryRegion = Region.Europe, Neighbors = new List<string> { "DE", "PL", "SK", "AT" } } },
            { "SK", new Country { IsoCode = "SK", CurrencyCode = "EUR", CountryRegion = Region.Europe, Neighbors = new List<string> { "CZ", "PL", "UA", "HU", "AT" } } },
            { "HU", new Country { IsoCode = "HU", CurrencyCode = "HUF", CountryRegion = Region.Europe, Neighbors = new List<string> { "SK", "UA", "RO", "RS", "HR", "SI", "AT" } } },

            // Northern Europe
            { "NO", new Country { IsoCode = "NO", CurrencyCode = "NOK", CountryRegion = Region.Europe, Neighbors = new List<string> { "SE", "FI", "RU", "IS", "DK", "GB" } } },
            { "SE", new Country { IsoCode = "SE", CurrencyCode = "SEK", CountryRegion = Region.Europe, Neighbors = new List<string> { "NO", "FI", "DK" } } },
            { "FI", new Country { IsoCode = "FI", CurrencyCode = "EUR", CountryRegion = Region.Europe, Neighbors = new List<string> { "SE", "NO", "RU", "EE" } } },
            { "DK", new Country { IsoCode = "DK", CurrencyCode = "DKK", CountryRegion = Region.Europe, Neighbors = new List<string> { "DE", "SE", "NO", "IS", "GB" } } },
            
            // Baltics
            { "EE", new Country { IsoCode = "EE", CurrencyCode = "EUR", CountryRegion = Region.Europe, Neighbors = new List<string> { "LV", "RU", "FI" } } },
            { "LV", new Country { IsoCode = "LV", CurrencyCode = "EUR", CountryRegion = Region.Europe, Neighbors = new List<string> { "EE", "LT", "RU", "BY" } } },
            { "LT", new Country { IsoCode = "LT", CurrencyCode = "EUR", CountryRegion = Region.Europe, Neighbors = new List<string> { "LV", "BY", "PL", "RU" } } },

            // --- CIS (Extended) ---
            { "KZ", new Country { IsoCode = "KZ", CurrencyCode = "KZT", CountryRegion = Region.CIS, Neighbors = new List<string> { "RU", "CN", "KG", "UZ", "TM" } } },
            { "UZ", new Country { IsoCode = "UZ", CurrencyCode = "UZS", CountryRegion = Region.CIS, Neighbors = new List<string> { "KZ", "KG", "TJ", "AF", "TM" } } },
            { "GE", new Country { IsoCode = "GE", CurrencyCode = "GEL", CountryRegion = Region.CIS, Neighbors = new List<string> { "RU", "TR", "AM", "AZ" } } },
            { "AM", new Country { IsoCode = "AM", CurrencyCode = "AMD", CountryRegion = Region.CIS, Neighbors = new List<string> { "GE", "AZ", "IR", "TR" } } },
            { "AZ", new Country { IsoCode = "AZ", CurrencyCode = "AZN", CountryRegion = Region.CIS, Neighbors = new List<string> { "RU", "GE", "AM", "IR", "TR" } } },
            
            // --- AMERICAS ---
            { "US", new Country { IsoCode = "US", CurrencyCode = "USD", CountryRegion = Region.Americas, Neighbors = new List<string> { "CA", "MX", "CU", "BS" } } },
            { "CA", new Country { IsoCode = "CA", CurrencyCode = "CAD", CountryRegion = Region.Americas, Neighbors = new List<string> { "US", "GL" } } },
            { "MX", new Country { IsoCode = "MX", CurrencyCode = "MXN", CountryRegion = Region.Americas, Neighbors = new List<string> { "US", "GT", "BZ", "CU" } } },
            
            // --- ASIA ---
            { "CN", new Country { IsoCode = "CN", CurrencyCode = "CNY", CountryRegion = Region.Asia, Neighbors = new List<string> { "RU", "MN", "KP", "VN", "LA", "MM", "IN", "BT", "NP", "PK", "AF", "TJ", "KG", "KZ" } } },
            { "JP", new Country { IsoCode = "JP", CurrencyCode = "JPY", CountryRegion = Region.Asia, Neighbors = new List<string> { "KR", "RU", "CN", "TW" } } },
            { "KR", new Country { IsoCode = "KR", CurrencyCode = "KRW", CountryRegion = Region.Asia, Neighbors = new List<string> { "KP", "JP", "CN" } } },
            { "TR", new Country { IsoCode = "TR", CurrencyCode = "TRY", CountryRegion = Region.Asia, Neighbors = new List<string> { "GR", "BG", "GE", "AM", "AZ", "IR", "IQ", "SY" } } },
            
            // --- OCEANIA ---
            { "AU", new Country { IsoCode = "AU", CurrencyCode = "AUD", CountryRegion = Region.Oceania, Neighbors = new List<string> { "NZ", "ID", "PG", "SB", "VU" } } },
            { "NZ", new Country { IsoCode = "NZ", CurrencyCode = "NZD", CountryRegion = Region.Oceania, Neighbors = new List<string> { "AU", "FJ", "TO" } } }
        };
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
                CurrencyCode = "USD",
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
            if (currencies.Count < 3) currencies.Add("USD");
            if (currencies.Count < 3) currencies.Add("EUR");
            if (currencies.Count < 3) currencies.Add("GBP");

            // Always enforce global anchor currencies availability
            currencies.Add("USD");
            currencies.Add("EUR");

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