using System;
using CONVERTinator.Domain;

namespace CONVERTinator.Helpers
{
    /// <summary>
    /// N.O.R.M.A. (Normalized Objective Rate Math Aggregator)
    /// Rate data and normalize it to a standard (1 USD = X) 
    /// with 3 decimal places.
    /// </summary>
    public static class RateNormalizer
    {
        public static Currency NormalizeToUsd(Currency rawCurrency, decimal usdCrossRate = 1m)
        {
            // ASSESSMENT (Data validation)
            // If the bank sent a negative rate or zero — discard it
            if (rawCurrency.Value <= 0 || usdCrossRate <= 0)
            {
                Console.WriteLine($"[N.O.R.M.A. Drop]: Invalid data for {rawCurrency.Code}");
                return null;
            }

            // CONVERSION (To the base currency USD)
            // If the data came, for example, in Euro (where 1 EUR = 1.08 USD), we divide the rate by 1.08.
            // If the data is already in USD, usdCrossRate is simply 1m.
            decimal exactValueInUsd = rawCurrency.Value / usdCrossRate;

            return new Currency
            {
                Code = rawCurrency.Code.ToUpper(),
                Name = rawCurrency.Name,
                Value = finalValue,
                Source = rawCurrency.Source
            };
        }
    }
}