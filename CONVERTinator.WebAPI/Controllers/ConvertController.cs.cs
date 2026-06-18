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

                if (rates.Count == 0)
                {
                    return StatusCode(500, new { error = "Database cache is empty. Run core app first." });
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
        public async Task<IActionResult> CalculateMultiple(string baseCur, string targetCurs, decimal amount)
        {
            try
            {
                var dbRepository = new DbRepository();
                var rates = await dbRepository.GetCachedRatesAsync();

                if (rates.Count == 0) return StatusCode(500, new { error = "Database cache is empty." });

                // Brake string of target currencies into a list, trim spaces, convert to uppercase and remove duplicates
                var targets = targetCurs.Split(',', StringSplitOptions.RemoveEmptyEntries)
                                        .Select(c => c.Trim().ToUpper())
                                        .Distinct()
                                        .ToList();

                // MAX 10 CURRENCIES!!!
                if (targets.Count > 10)
                {
                    return BadRequest(new { error = "Maximum 10 target currencies allowed per request." });
                }

                // Results
                var results = new List<object>();

                foreach (var target in targets)
                {
                    decimal? converted = MedianCalculator.Convert(amount, baseCur.ToUpper(), target, rates);

                    results.Add(new
                    {
                        currency = target,
                        rateFound = converted != null,
                        result = converted != null ? Math.Round(converted.Value, 3) : (decimal?)null
                    });
                }

                return Ok(new
                {
                    baseCurrency = baseCur.ToUpper(),
                    originalAmount = amount,
                    conversions = results
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }
    }
}