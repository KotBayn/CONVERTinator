using System.Collections.Generic;
using FluentAssertions;
using Xunit;
using CONVERTinator.Helpers;
using CONVERTinator.Domain;
using CONVERTinator.Domain.Entities;

namespace CONVERTinator.Tests.Helpers
{
    public class RateNormalizerTests
    {
        [Fact]
        public void NormalizeForeignBase_WithValidData_ReturnsCorrectCurrencyAndMath()
        {
            // Arrange
            // Example: We got a raw rate of 120 (let's say for testing) and USD cross rate is 1.2
            var rawCurrency = new Currency { Code = "gbp", Name = "Pound", Value = 120m, Source = "ECB" };
            decimal usdCrossRate = 1.2m;

            // Act: 120 / 1.2 = 100
            var result = RateNormalizer.NormalizeForeignBase(rawCurrency, usdCrossRate);

            // Assert
            result.Should().NotBeNull();
            result.Code.Should().Be("GBP");
            result.Value.Should().Be(100m);
            result.Name.Should().Be("Pound");
        }

        [Theory]
        [InlineData(0, 1.2)]     // Zero raw value
        [InlineData(-10, 1.2)]   // Negative raw value
        [InlineData(120, 0)]     // Zero cross rate
        [InlineData(120, -1.2)]  // Negative cross rate
        public void NormalizeForeignBase_WithZeroOrNegativeValues_ReturnsNull(decimal rawValue, decimal crossRate)
        {
            // Arrange
            var rawCurrency = new Currency { Code = "EUR", Value = rawValue };

            // Act
            var result = RateNormalizer.NormalizeForeignBase(rawCurrency, crossRate);

            // Assert
            result.Should().BeNull();
        }

        [Fact]
        public void NormalizeLocalBase_WithValidData_ReturnsInvertedMath()
        {
            // Arrange
            // Example: In Poland, 1 USD costs 4 PLN. Some other currency costs 2 PLN.
            var rawCurrency = new Currency { Code = "czk", Name = "Koruna", Value = 2m, Source = "NBP" };
            decimal usdRateInLocal = 4m;

            // Act: 4 / 2 = 2
            var result = RateNormalizer.NormalizeLocalBase(rawCurrency, usdRateInLocal);

            // Assert
            result.Should().NotBeNull();
            result.Code.Should().Be("CZK");
            result.Value.Should().Be(2m);
        }

        [Theory]
        [InlineData(0, 4.0)]     // Zero raw value
        [InlineData(-5, 4.0)]    // Negative raw value
        [InlineData(2, 0)]       // Zero local USD rate
        [InlineData(2, -4.0)]    // Negative local USD rate
        public void NormalizeLocalBase_WithZeroOrNegativeValues_ReturnsNull(decimal rawValue, decimal usdRate)
        {
            // Arrange
            var rawCurrency = new Currency { Code = "CZK", Value = rawValue };

            // Act
            var result = RateNormalizer.NormalizeLocalBase(rawCurrency, usdRate);

            // Assert
            result.Should().BeNull();
        }
    }
}