using System;
using CONVERTinator.Domain.Entities;

namespace CONVERTinator.Helpers
{
    /// <summary>
    /// N.O.R.M.A. (Normalized Objective Rate Math Aggregator)
    /// Rate data and normalize it to a standard with 3 decimal places.
    /// </summary>
    public static class RateNormalizer
    {
        /// <summary>
        /// Using for ECB (Germany) and other banks where the base currency is foreign (e.g., EUR).
        /// </summary>
        public static Currency? NormalizeForeignBase(Currency rawCurrency, decimal usdCrossRate)
        {
            if (rawCurrency.Value <= 0 || usdCrossRate <= 0) return null;

            decimal exactValueInUsd = rawCurrency.Value / usdCrossRate;

            return CreateCurrency(rawCurrency, exactValueInUsd);
        }

        /// <summary>
        /// Using for countries where the API provides prices in LOCAL currency (PLN, UAH, CZK).
        /// </summary>
        public static Currency? NormalizeLocalBase(Currency rawCurrency, decimal usdRateInLocalCurrency)
        {
            if (rawCurrency.Value <= 0 || usdRateInLocalCurrency <= 0) return null;

            // INVERTED MATH (Dollar rate divided by currency rate)
            decimal exactValueInUsd = usdRateInLocalCurrency / rawCurrency.Value;

            return CreateCurrency(rawCurrency, exactValueInUsd);
        }

        // Helper method to avoid duplicating object creation code
        private static Currency CreateCurrency(Currency raw, decimal exactValue)
        {
            return new Currency
            {
                Code = raw.Code.ToUpper(),
                Name = raw.Name,
                Value = exactValue,
                Source = raw.Source
            };
        }
    }
}