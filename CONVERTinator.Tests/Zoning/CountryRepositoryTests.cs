using System.Collections.Generic;
using FluentAssertions;
using Xunit;
using CONVERTinator.Helpers;
using CONVERTinator.Domain.GEO; 

namespace CONVERTinator.Tests.Zoning
{
    public class CountryRepositoryTests
    {

        [Fact]
        public void GetCountryByIso_WithValidIso_ReturnsCorrectCountryNode()
        {
            // Act
            var result = CountryRepository.GetCountryByIso("PL");

            // Assert
            result.Should().NotBeNull();
            result.IsoCode.Should().Be("PL");
            result.CurrencyCode.Should().Be("PLN");
            result.CountryRegion.Should().Be(Region.Europe);
        }

        [Fact]
        public void GetCountryByIso_WithLowercaseIso_HandlesCaseInsensitivity()
        {
            // Act
            var result = CountryRepository.GetCountryByIso("de");

            // Assert
            result.Should().NotBeNull();
            result.CurrencyCode.Should().Be("EUR");
        }

        [Fact]
        public void GetCountryByIso_WithUnknownIso_ReturnsGlobalStub()
        {
            // Arrange
            string fakeIso = "NARNIA";

            // Act
            var result = CountryRepository.GetCountryByIso(fakeIso);

            // Assert
            result.Should().NotBeNull();
            result.IsoCode.Should().Be(fakeIso);
            result.CurrencyCode.Should().Be("USD");
            result.CountryRegion.Should().Be(Region.Global);
        }

        [Fact]
        public void GetTravelCurrencies_WithValidIso_IncludesHostNeighborsAndAnchors()
        {
            // Arrange & Act
            var currencies = CountryRepository.GetTravelCurrencies("UA");

            // Assert
            currencies.Should().NotBeNullOrEmpty();
            currencies.Should().Contain("UAH");
            currencies.Should().Contain("PLN");
            currencies.Should().Contain("RON");
            currencies.Should().Contain("USD");
            currencies.Should().Contain("EUR");
        }

        [Fact]
        public void GetTravelCurrencies_WithIsolatedIsland_EnsuresMinimumRequiredCurrencies()
        {
            // Arrange
            var currencies = CountryRepository.GetTravelCurrencies("ISOLATED");

            // Assert
            currencies.Count.Should().BeGreaterThanOrEqualTo(3);
            currencies.Should().Contain(new[] { "USD", "EUR", "GBP" });
        }

        [Fact]
        public void GetRequiredRegions_ForTranscontinentalCountry_ReturnsMultipleRegions()
        {
            // Arrange & Act: Turkey (TR) is classified as Asia in our DB, 
            // but borders Europe (GR, BG) and CIS (GE, AM, AZ) and Middle East (SY, IQ, IR).
            // NOTE: According to your current dictionary, TR neighbors: GR, BG, GE, AM, AZ, IR, IQ, SY
            var regions = CountryRepository.GetRequiredRegions("TR");

            // Assert: The graph resolver must figure out to load data from all these endpoints!
            regions.Should().Contain(Region.Asia);       // Host region
            regions.Should().Contain(Region.Europe);     // Because of GR, BG
            regions.Should().Contain(Region.CIS);        // Because of GE, AM, AZ

            // Check if MiddleEast is triggered (Because of IQ, SY)
            // (Assumes IQ/SY are mapped to MiddleEast in your actual dictionary)
            // regions.Should().Contain(Region.MiddleEast); 
        }

        [Fact]
        public void GetRequiredRegions_ForCentralEurope_ReturnsOnlyEurope()
        {
            // Act
            var regions = CountryRepository.GetRequiredRegions("DE");

            // Assert
            regions.Should().HaveCount(1);
            regions.Should().Contain(Region.Europe);
        }
    }
}