using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using CONVERTinator.Repositories;
using CONVERTinator.Helpers;

namespace CONVERTinator.WebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")] 
    public class ConvertController : ControllerBase
    {
        // method to calculate exchange rate between two currencies
        [HttpGet("exchange")]
        [ProducesResponseType(200)] 
        [ProducesResponseType(400)] 
        [ProducesResponseType(500)]
        public async Task<IActionResult> CalculateExchange(string baseCur, string targetCur, decimal amount)
        {
            try
            {
                var dbRepository = new DbRepository();
                var rates = await dbRepository.GetCachedRatesAsync();
                bool isFresh = await dbRepository.IsCacheFreshAsync(TimeSpan.FromHours(2));

                // AUTO-REFRESH LOGIC
                if (rates.Count == 0 || !isFresh)
                {
                    var sync = new CONVERTinator.Services.CacheSyncService();
                    await sync.ForceUpdateAsync();

                    rates = await dbRepository.GetCachedRatesAsync();
                }

                if (rates.Count == 0)
                {
                    return StatusCode(500, new { error = "Global network failure. Providers did not respond." });
                }

                // Using MedianCalculator to convert the amount from baseCur to targetCur
                decimal? result = MedianCalculator.Convert(amount, baseCur.ToUpper(), targetCur.ToUpper(), rates);

                if (result == null)
                {
                    return NotFound(new { error = $"Conversion path between {baseCur} and {targetCur} not found." });
                }

                return Ok(new
                {
                    status = "success",
                    baseCurrency = baseCur.ToUpper(),
                    targetCurrency = targetCur.ToUpper(),
                    originalAmount = amount,
                    convertedAmount = Math.Round(result.Value, 3),
                    timestamp = DateTime.UtcNow
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpGet("multi")]
        public async Task<IActionResult> CalculateMultiple(string baseCur, [FromQuery] List<string> targetCurs, decimal amount)
        {
            try
            {
                var dbRepository = new DbRepository();
                var rates = await dbRepository.GetCachedRatesAsync();
                bool isFresh = await dbRepository.IsCacheFreshAsync(TimeSpan.FromHours(2));

                // Auto-refresh logic
                if (rates.Count == 0 || !isFresh)
                {
                    var sync = new CONVERTinator.Services.CacheSyncService();
                    await sync.ForceUpdateAsync();

                    rates = await dbRepository.GetCachedRatesAsync();
                }

                if (rates.Count == 0)
                {
                    return StatusCode(500, new { error = "Global network failure. Providers did not respond." });
                }
                // Trim and uppercase target currencies, remove duplicates
                var targets = targetCurs
                                .Where(c => !string.IsNullOrWhiteSpace(c))
                                .Select(c => c.Trim().ToUpper())
                                .Distinct()
                                .ToList();

                // MAX 10 CURRENCIES!!!
                if (targets.Count > 10)
                {
                    return BadRequest(new { error = "Maximum 10 target currencies allowed per request." });
                }

                var results = new List<object>();

                foreach (var target in targets)
                {
                    decimal? converted = MedianCalculator.Convert(amount, baseCur.ToUpper(), target, rates);

                    results.Add(new
                    {
                        targetCurrency = target,
                        convertedAmount = converted != null ? Math.Round(converted.Value, 4) : (decimal?)null,
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
                return BadRequest(new { status = "error", error = ex.Message });
            }
        }
    }
}