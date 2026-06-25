using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Xunit;
using CONVERTinator.Data;
using CONVERTinator.Domain;
using CONVERTinator.Domain.Entities;
using CONVERTinator.Repositories;

namespace CONVERTinator.Tests.DB
{
    public class DbRepositoryTests : IDisposable
    {
        private readonly DbContextOptions<AppDbContext> _options;
        private readonly DbRepository _repository;
        private readonly AppDbContext _context;

        // SETUP 
        public DbRepositoryTests()
        {
            string dbName = Guid.NewGuid().ToString();

            _options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: dbName) 
                .Options;

            _repository = new DbRepository(_options);
            _context = new AppDbContext(_options);
            _context.Database.EnsureCreated();
        }

        // TEARDOWN 
        public void Dispose()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }

        [Fact]
        public async Task SaveSettingsAsync_WhenCalled_CreatesAndFormatsSettings()
        {
            // Act
            var activeCurrencies = new List<string> { "eur", " gbp " };
            await _repository.SaveSettingsAsync("USD", activeCurrencies);

            // Assert
            var settings = await _context.Settings.FirstOrDefaultAsync();

            settings.Should().NotBeNull();
            settings.BaseCurrency.Should().Be("USD");
            settings.SavedCurrencies.Should().Be("EUR,GBP");
        }

        [Fact]
        public async Task SaveRatesAsync_WhenCalled_ReplacesOldRatesWithNew()
        {
            // Arrange
            _context.CachedRates.Add(new CachedRate
            {
                CurrencyCode = "OLD",
                UsdRate = 1.0m,
                Source = "Bank",
                FetchTime = DateTime.UtcNow
            });
            await _context.SaveChangesAsync();

            var newRates = new List<Currency>
            {
                new Currency { Code = "NEW", Value = 5.0m, Source = "NewBank" }
            };

            // Act
            await _repository.SaveRatesAsync(newRates);

            // Assert
            var ratesInDb = await _context.CachedRates.ToListAsync();
            ratesInDb.Should().HaveCount(1);
            ratesInDb.First().CurrencyCode.Should().Be("NEW");
        }

        [Fact]
        public async Task IsCacheFreshAsync_WithStaleCache_ReturnsFalse()
        {
            // Arrange
            _context.CachedRates.Add(new CachedRate
            {
                CurrencyCode = "USD",
                UsdRate = 1.0m,
                Source = "Bank",
                FetchTime = DateTime.UtcNow.AddHours(-2)
            });
            await _context.SaveChangesAsync();

            // Act
            bool isFresh = await _repository.IsCacheFreshAsync(TimeSpan.FromHours(1));

            // Assert
            isFresh.Should().BeFalse();
        }
    }
}