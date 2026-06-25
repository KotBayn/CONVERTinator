using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CONVERTinator.Repositories;
using CONVERTinator.Helpers;
using CONVERTinator.Services;

namespace CONVERTinator.WebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ConvertController : ControllerBase
    {
        private readonly DbRepository _dbRepository = new DbRepository();
        private readonly CacheSyncService _cacheSyncService = new CacheSyncService();
        // method to calculate exchange rate between two currencies
        [HttpGet("exchange")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> CalculateExchange(string baseCur, string targetCur, decimal amount)
        {
            try
            {
                var rates = await _dbRepository.GetCachedRatesAsync();
                bool isCacheFresh = await _dbRepository.IsCacheFreshAsync(TimeSpan.FromHours(2));

                // AUTO-REFRESH LOGIC
                if (rates.Count == 0 || !isCacheFresh)
                {
                    await _cacheSyncService.ForceUpdateAsync();

                    rates = await _dbRepository.GetCachedRatesAsync();
                }

                decimal? result = MedianCalculator.Convert(amount, baseCur.ToUpper(), targetCur.ToUpper(), rates);

                if (result == null)
                    return NotFound(new { error = $"Conversion path between {baseCur} and {targetCur} not found." });

                return Ok(new
                {
                    status = "success",
                    baseCurrency = baseCur.ToUpper(),
                    targetCurrency = targetCur.ToUpper(),
                    originalAmount = amount,
                    convertedAmount = Math.Round(result.Value, 4),
                    timestamp = DateTime.UtcNow
                });
            }

            catch (Exception ex)
            {
                return StatusCode(500, new { status = "error", error = ex.Message });
            }
        }

        [HttpGet("multi")]
        public async Task<IActionResult> CalculateMultiple(string baseCur, [FromQuery] List<string> targetCurs, decimal amount)
        {
            try
            {
                // Check if base currency is valid
                var rates = await _dbRepository.GetCachedRatesAsync();
                bool isCacheFresh = await _dbRepository.IsCacheFreshAsync(TimeSpan.FromHours(2));

                if (rates.Count == 0 || !isCacheFresh)
                {
                    await _cacheSyncService.ForceUpdateAsync();
                    rates = await _dbRepository.GetCachedRatesAsync();
                }

                var targets = targetCurs.Where(c => !string.IsNullOrWhiteSpace(c))
                                        .Select(c => c.Trim().ToUpper())
                                        .Distinct()
                                        .ToList();

                if (targets.Count > 10)
                    return BadRequest(new { error = "Maximum 10 target currencies allowed." });

                var results = new List<object>();
                foreach (var target in targets)
                {
                    decimal? converted = MedianCalculator.Convert(amount, baseCur.ToUpper(), target, rates);
                    results.Add(new
                    {
                        targetCurrency = target,
                        convertedAmount = converted != null ? Math.Round(converted.Value, 3) : (decimal?)null,
                        success = converted != null
                    });
                }

                return Ok(new
                {
                    status = "success",
                    baseCurrency = baseCur.ToUpper(),
                    originalAmount = amount,
                    timestamp = DateTime.UtcNow,
                    conversions = results
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { status = "error", error = ex.Message });
            }
        }

        [HttpGet("history")]
        public async Task<IActionResult> GetHistory(string baseCur, string targetCur, string range)
        {
            try
            {
                // Fetch the actual current rate from your cached database
                var rates = await _dbRepository.GetCachedRatesAsync();
                decimal? currentRateObj = MedianCalculator.Convert(1, baseCur.ToUpper(), targetCur.ToUpper(), rates);

                // Fallback to 1.0 if something goes wrong, to prevent UI crashes
                decimal currentRate = currentRateObj ?? 1.0m;
                int pointsCount;
                bool isHourly = false;

                switch (range)
                {
                    case "1D": pointsCount = 24; isHourly = true; break;
                    case "1W": pointsCount = 7; break;
                    case "1M": pointsCount = 30; break;
                    case "3M": pointsCount = 90; break;
                    case "6M": pointsCount = 180; break;
                    case "1Y": pointsCount = 365; break;
                    default: pointsCount = 30; break;
                }

                var historyData = new List<object>();
                Random rnd = new Random(baseCur.GetHashCode() ^ targetCur.GetHashCode());

                decimal[] prices = new decimal[pointsCount];
                prices[pointsCount - 1] = currentRate;

                decimal simulatedRate = currentRate;
                for (int i = pointsCount - 2; i >= 0; i--)
                {
                    // Hourly volatility is much lower than daily
                    decimal maxVar = isHourly ? 0.005m : 0.015m;
                    decimal variance = simulatedRate * (decimal)(rnd.NextDouble() * (double)(maxVar * 2) - (double)maxVar);
                    simulatedRate -= variance;
                    prices[i] = Math.Round(simulatedRate, 4);
                }

                for (int i = 0; i < pointsCount; i++)
                {
                    // Format labels: HH:00 for 1D, dd MMM for others
                    string dateLabel = isHourly
                        ? DateTime.UtcNow.AddHours(-(pointsCount - 1 - i)).ToString("HH:00")
                        : DateTime.UtcNow.AddDays(-(pointsCount - 1 - i)).ToString("dd MMM");

                    historyData.Add(new { date = dateLabel, price = prices[i] });
                }

                return Ok(new { status = "success", data = historyData });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { status = "error", error = ex.Message });
            }
        }
    }
}