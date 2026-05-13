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
        private readonly HttpClient _httpClient = new HttpClient();

        public async Task<string> TryGetIsoCodeAsync()
        {
            try
            {
                // In a MAUI/Mobile app, retrieve real device coordinates:
                // var location = await Geolocation.GetLastKnownLocationAsync();
                // double latitude = location.Latitude;
                // double longitude = location.Longitude;

                // Using hardware mock coordinates for console testing
                double latitude = 52.16;
                double longitude = 20.80;

                // OpenStreetMap (Nominatim API) requires a User-Agent header
                _httpClient.DefaultRequestHeaders.Add("User-Agent", "CONVERTinator");
                _httpClient.Timeout = TimeSpan.FromSeconds(3);

                // Reverse geocoding request
                string url = $"https://nominatim.openstreetmap.org/reverse?format=json&lat={latitude}&lon={longitude}";
                var response = await _httpClient.GetStringAsync(url);

                using JsonDocument doc = JsonDocument.Parse(response);

                if (doc.RootElement.TryGetProperty("address", out JsonElement addressElement) &&
                    addressElement.TryGetProperty("country_code", out JsonElement countryCodeElement))
                {
                    return countryCodeElement.GetString().ToUpper();
                }

                return null;
            }
            catch
            {
                return null;
            }
        }
    }

    // Culture Fallback Provider
    public class CultureLocationProvider : ILocationProvider
    {
        public Task<string> TryGetIsoCodeAsync()
        {
            try
            {
                // Extracts country code from regional formatting (e.g., "en-US" -> "US")
                var culture = CultureInfo.CurrentCulture;

                if (culture.Name.Length >= 2)
                {
                    var parts = culture.Name.Split('-');
                    if (parts.Length == 2)
                    {
                        return Task.FromResult(parts[1].ToUpper());
                    }
                }

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
            // The exact order of execution (Chain of Responsibility)
            _providers = new List<ILocationProvider>
            {
                new IpLocationProvider(),              // 1. Network IP Check
                new MobileOperatorLocationProvider(),  // 2. Cellular Tower MCC
                new GpsLocationProvider(),             // 3. GPS + Reverse Geocoding
                new SystemLocaleLocationProvider(),    // 4. OS Geo Region
                new CultureLocationProvider()          // 5. OS Formatting Culture
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