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
    }
}