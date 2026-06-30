using CONVERTinator.Domain;
using CONVERTinator.Domain.GEO;
using CONVERTinator.Domain.Interfaces;
using CONVERTinator.Helpers;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace CONVERTinator.WebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LocationController : ControllerBase
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;

        public LocationController(IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
        }

        [HttpGet("current")]
        public async Task<IActionResult> GetCurrentLocation([FromQuery] string overrideIso = null)
        {
            string isoCode = Constants.MainCurrency.MainISO; // Default fallback country code

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
                    string baseUrl = _configuration["ExternalApis:IpGeolocationBaseUrl"];

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
                    var client = _httpClientFactory.CreateClient();
                    string response = await client.GetStringAsync(apiUrl);
                    using var doc = JsonDocument.Parse(response);

                    if (doc.RootElement.TryGetProperty("countryCode", out var codeProp))
                    {
                        isoCode = codeProp.GetString() ?? Constants.MainCurrency.MainISO;
                    }
                }
                catch (Exception)
                {
                    isoCode = Constants.MainCurrency.MainISO;
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