using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using Xunit;
using CONVERTinator.Helpers;
using CONVERTinator.Domain.Entities;

namespace CONVERTinator.Tests.Helpers
{
    public class MedianCalculatorTests
    {

        [Theory]
        [InlineData(new double[] { 10, 20, 30 }, 20)]       // Odd number of elements
        [InlineData(new double[] { 10, 20, 30, 40 }, 25)]   // Even number (average of 20 and 30)
        [InlineData(new double[] { 50 }, 50)]               // Single element
        public void Calculate_WithValidValues_ReturnsCorrectMedian(double[] inputs, decimal expected)
        {
            // Arrange
            var list = inputs.Select(x => (decimal)x).ToList();

            // Act
            decimal result = MedianCalculator.Calculate(list);

            // Assert
            result.Should().Be(expected);
        }

        [Fact]
        public void Calculate_WithNullOrEmptyList_ReturnsZero()
        {
            // Act & Assert
            MedianCalculator.Calculate(null).Should().Be(0m);
            MedianCalculator.Calculate(new List<decimal>()).Should().Be(0m);
        }

        [Fact]
        public void Convert_WithDifferentCurrencies_CalculatesCrossRateUsingMedian()
        {
            // Arrange
            var rates = new List<Currency>
            {
                // We have 3 PLN rates. The median of (3.9, 4.0, 4.1) is 4.0.
                // Meaning: 1 USD = 4.0 PLN
                new Currency { Code = "PLN", Value = 3.9m },
                new Currency { Code = "PLN", Value = 4.1m },
                new Currency { Code = "PLN", Value = 4.0m },
                
                // We have 2 EUR rates. The median of (0.8, 0.8) is 0.8.
                // Meaning: 1 USD = 0.8 EUR
                new Currency { Code = "EUR", Value = 0.8m },
                new Currency { Code = "EUR", Value = 0.8m }
            };

            // Act
            // Internal Math: 
            // 1. amountInUsd = 100 PLN / 4.0 = 25 USD
            // 2. result = 25 USD * 0.8 EUR = 20 EUR
            decimal? result = MedianCalculator.Convert(100m, "PLN", "EUR", rates);

            // Assert
            result.Should().NotBeNull();
            result.Should().Be(20.000m); // The method rounds to 3 decimal places
        }

        [Fact]
        public void Convert_SameCurrency_ReturnsAmountRoundedToThreeDecimals()
        {
            // Act
            decimal? result = MedianCalculator.Convert(100.1239m, "EUR", "EUR", new List<Currency>());

            // Assert
            result.Should().Be(100.124m); // Must round up due to '9'
        }

        [Fact]
        public void Convert_WithUsdAsBase_CalculatesCorrectly()
        {
            // Arrange
            var rates = new List<Currency> { new Currency { Code = "EUR", Value = 0.8m } };

            // Act
            decimal? result = MedianCalculator.Convert(100m, "USD", "EUR", rates);

            // Assert
            result.Should().Be(80.000m);
        }

        [Fact]
        public void Convert_WithMissingTargetCurrency_ReturnsNull()
        {
            // Arrange
            var rates = new List<Currency> { new Currency { Code = "PLN", Value = 4.0m } };

            // Act
            decimal? result = MedianCalculator.Convert(100m, "PLN", "EUR", rates);

            // Assert
            result.Should().BeNull();
        }
    }
}