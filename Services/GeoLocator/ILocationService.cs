using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using System.Globalization;

namespace CONVERTinator.Services.GeoLocator
{
    // The main interface for location service
    public interface ILocationService
    {
        Task<string> GetCurrentCountryIsoCodeAsync();
    }

    // Interface for each specific location strategy
    public interface ILocationProvider
    {
        Task<string> TryGetIsoCodeAsync();
    }

    // IP Provider
    public class IpLocationProvider : ILocationProvider
    {
        private readonly HttpClient _httpClient = new HttpClient();

        public async Task<string> TryGetIsoCodeAsync()
        {
            try
            {
                _httpClient.Timeout = TimeSpan.FromSeconds(3);

                // Using a free IP geolocation API
                var response = await _httpClient.GetStringAsync("http://ip-api.com/json/");
                using JsonDocument doc = JsonDocument.Parse(response);

                if (doc.RootElement.TryGetProperty("countryCode", out JsonElement codeElement))
                {
                    return codeElement.GetString();
                }

                return null;
            }
            catch
            {
                // Return null if there is no internet or the API is down
                return null;
            }
        }
    }

    // Mobile Operator Provider
    public class MobileOperatorLocationProvider : ILocationProvider
    {
        public Task<string> TryGetIsoCodeAsync()
        {
            try
            {
                // In a Android/MAUI app, will query the Telephony API here.
                // For a console application, return null to trigger the fallback.
                return Task.FromResult<string>(null);
            }
            catch
            {
                return Task.FromResult<string>(null);
            }
        }
    }

    // System Locale Provider
    public class SystemLocaleLocationProvider : ILocationProvider
    {
        public Task<string> TryGetIsoCodeAsync()
        {
            try
            {
                var region = RegionInfo.CurrentRegion;
                return Task.FromResult(region.TwoLetterISORegionName);
            }
            catch
            {
                return Task.FromResult<string>(null);
            }
        }
    }

    // GPS Provider
    public class GpsLocationProvider : ILocationProvider
    {
        public Task<string> TryGetIsoCodeAsync()
        {
            try
            {
                // GPS requires hardware access and location permissions.
                // Returning null to complete the chain demonstration.
                return Task.FromResult<string>(null);
            }
            catch
            {
                return Task.FromResult<string>(null);
            }
        }
    }

    // The orchestrator that chains all providers together
    public class LocationService : ILocationService
    {
        private readonly List<ILocationProvider> _providers;

        public LocationService()
        {
            _providers = new List<ILocationProvider>
            {
                new IpLocationProvider(),
                new MobileOperatorLocationProvider(),
                new SystemLocaleLocationProvider(),
                new GpsLocationProvider()
            };
        }

        public async Task<string> GetCurrentCountryIsoCodeAsync()
        {
            foreach (var provider in _providers)
            {
                string isoCode = await provider.TryGetIsoCodeAsync();

                if (!string.IsNullOrWhiteSpace(isoCode))
                {
                    return isoCode.ToUpper();
                }
            }

            // Ultimate fallback if absolutely everything fails
            return "US";
        }
    }
}