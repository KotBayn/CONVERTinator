using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CONVERTinator.Domain
{
    public static class Constants
    {
        public static class Cache
        {
            public const int CacheExpirationHours = 2; // Cache expiration time in hours
        }
        
        public static class Validation
        {
            public const int MaxCurrencyCodeLength = 3; // Maximum length for currency codes (e.g., USD, EUR)
            public const int MinCurrencyCodeLength = 3; // Minimum length for currency codes
            public const int MaxTargetCurrencyLength = 10; // Maximum length for target currency codes
            public const decimal MinAmount = 0.01m; // Minimum amount for conversion
            public const decimal MaxAmount = 1000000m; // Maximum amount for conversion
        }

        public static class MainCurrency
        {
            public const string USD = "USD"; // Base/Default currency for conversions
            public const string MainISO = "US"; // Default ISO country code for the main currency
            public const string EUR = "EUR"; // Example of another main currency
        }

        public static class ErrorMessages
        {
            public const string InvalidCurrencyCode = "Invalid currency code. Currency codes must be 3 uppercase letters.";
            public const string InvalidAmount = "Invalid amount. Amount must be a positive number between 0.01 and 1,000,000.";
            public const string ConversionPathNotFound = "Conversion path between {0} and {1} not found.";
            public const string CacheUpdateFailed = "Failed to update cache from the database.";
            public const string InvalidNumericFormat = "Invalid numeric format.";
            public const string UnknownCommand = "Unknown command.";
        }
    }
}
