using System.Collections.Generic;
using System.Linq;
using CONVERTinator.Domain;

namespace CONVERTinator.Helpers
{
    public static class MedianCalculator
    {
        /// <summary>
        /// Calculates the mathematical median from a list of decimal values.
        /// </summary>
        public static decimal Calculate(List<decimal> values)
        {
            if (values == null || values.Count == 0)
                return 0;

            var sortedValues = values.OrderBy(v => v).ToList();
            int midIndex = sortedValues.Count / 2;

            if (sortedValues.Count % 2 != 0)
            {
                return sortedValues[midIndex];
            }

            return (sortedValues[midIndex] + sortedValues[midIndex - 1]) / 2m;
        }

        /// <summary>
        /// Performs cross-rate conversion between two currencies using USD as the base anchor.
        /// Returns null if sufficient market data is not available.
        /// </summary>
        public static decimal? Convert(decimal amount, string baseCurrency, string targetCurrency, List<Currency> allRates)
        {
            if (baseCurrency == targetCurrency)
                return amount;

            // 1. Resolve base currency rate relative to USD
            decimal baseRateToUsd = baseCurrency == "USD" ? 1m : GetMedianForCurrency(baseCurrency, allRates);
            if (baseRateToUsd == 0m)
                return null; // Missing data for base currency

            // 2. Resolve target currency rate relative to USD
            decimal targetRateToUsd = targetCurrency == "USD" ? 1m : GetMedianForCurrency(targetCurrency, allRates);
            if (targetRateToUsd == 0m)
                return null; // Missing data for target currency

            // 3. Cross-rate calculation
            decimal amountInUsd = amount / baseRateToUsd;
            return amountInUsd * targetRateToUsd;
        }

        /// <summary>
        /// Helper method to extract and calculate the median rate for a specific currency code.
        /// </summary>
        private static decimal GetMedianForCurrency(string currencyCode, List<Currency> allRates)
        {
            var foundRates = allRates
                .Where(c => c.Code == currencyCode)
                .Select(c => c.Value)
                .ToList();

            return Calculate(foundRates);
        }
    }
}