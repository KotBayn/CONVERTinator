using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using Xunit;
using CONVERTinator.Services;
using CONVERTinator.Domain;

namespace CONVERTinator.Tests.Providers
{
    public class RegionProviderCompositeTests
    {
        [Fact]
        public async Task GetRatesAsync_ShouldAggregateRatesFromAllChildren()
        {
            // Arrange
            var composite = new RegionProviderComposite("Global Zone");

            // Fake provider for Bank 1
            var mockBank1 = new Mock<IExchangeRateProvider>();
            mockBank1.Setup(p => p.GetRatesAsync())
                     .ReturnsAsync(new List<Currency> { new Currency { Code = "EUR", Value = 0.9m } });

            // Fake provider for Bank 2
            var mockBank2 = new Mock<IExchangeRateProvider>();
            mockBank2.Setup(p => p.GetRatesAsync())
                     .ReturnsAsync(new List<Currency> { new Currency { Code = "GBP", Value = 0.7m } });

            composite.Add(mockBank1.Object);
            composite.Add(mockBank2.Object);

            // Act
            var result = await composite.GetRatesAsync();

            // Assert
            result.Should().HaveCount(2); 
            result.Should().ContainSingle(c => c.Code == "EUR");
            result.Should().ContainSingle(c => c.Code == "GBP");
        }
    }
}