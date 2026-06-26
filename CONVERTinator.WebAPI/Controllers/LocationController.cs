using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using CONVERTinator.Helpers;
using CONVERTinator.Domain.GEO;

namespace CONVERTinator.WebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LocationController : ControllerBase
    {
        private static readonly HttpClient _httpClient = new HttpClient();

        [HttpGet("current")]
        public async Task<IActionResult> GetCurrentLocation([FromQuery] string overrideIso = null)
        {
            string isoCode = "US"; // Default fallback country code

            if (!string.IsNullOrWhiteSpace(overrideIso))
            {
                isoCode = overrideIso.Trim().ToUpper();
            }
            else
            {
                try
                {
                    // Get the REAL user IP from Render's load balancer
                    string userIp = HttpContext.Request.Headers["X-Forwarded-For"].FirstOrDefault();

                    // Fallback for local development
                    if (string.IsNullOrEmpty(userIp))
                    {
                        userIp = HttpContext.Connection.RemoteIpAddress?.ToString();
                    }

                    if (!string.IsNullOrEmpty(userIp) && userIp.Contains(","))
                    {
                        userIp = userIp.Split(',')[0].Trim();
                    }

                    if (userIp == "::1" || userIp == "127.0.0.1" || userIp.Contains("localhost"))
                    {
                        userIp = "";
                    }

                    string apiUrl = string.IsNullOrEmpty(userIp)
                        ? "http://ip-api.com/json/"
                        : $"http://ip-api.com/json/{userIp}";

                    // Execute the request
                    var response = await _httpClient.GetStringAsync(apiUrl);
                    using var doc = JsonDocument.Parse(response);

                    if (doc.RootElement.TryGetProperty("countryCode", out var codeProp))
                    {
                        isoCode = codeProp.GetString() ?? "US";
                    }
                }
                catch (Exception)
                {
                    isoCode = "US";
                }
            }

            Country countryInfo = CountryRepository.GetCountryByIso(isoCode);

            return Ok(new
            {
                isoCode = countryInfo.IsoCode,
                currencyCode = countryInfo.CurrencyCode,
                region = countryInfo.CountryRegion.ToString() 
            });
        }
    }
}