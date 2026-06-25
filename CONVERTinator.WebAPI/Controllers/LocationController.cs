using Microsoft.AspNetCore.Mvc;
using System;
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

            // If a developer provided an override parameter, bypass the IP detection
            if (!string.IsNullOrWhiteSpace(overrideIso))
            {
                isoCode = overrideIso.Trim().ToUpper();
            }
            else
            {
                try
                {
                    // Call a free IP geolocation API to detect the public IP of the host
                    // This automatically target-resolves location during local testing
                    var response = await _httpClient.GetStringAsync("http://ip-api.com/json/");
                    using var doc = JsonDocument.Parse(response);

                    if (doc.RootElement.TryGetProperty("countryCode", out var codeProp))
                    {
                        isoCode = codeProp.GetString() ?? "US";
                    }
                }
                catch (Exception)
                {
                    // Fallback
                    isoCode = "US";
                }
            }

            Country countryInfo = CountryRepository.GetCountryByIso(isoCode);

            return Ok(new
            {
                isoCode = countryInfo.IsoCode,
                currencyCode = countryInfo.CurrencyCode,
                region = countryInfo.CountryRegion.ToString() // E.g., "Europe", "CIS", "Asia"
            });
        }
    }
}