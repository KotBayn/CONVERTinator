using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using System.Xml.Linq;
using CONVERTinator.Domain.Entities;
using CONVERTinator.Domain.Interfaces;

namespace CONVERTinator.Services.Regions.Europe.Providers.Romania
{
    public class BnrProvider : IExchangeRateProvider
    {
        private readonly HttpClient _httpClient = new HttpClient();
        private const string Url = "https://www.bnr.ro/nbrfxrates.xml"; // National Bank of Romania

        public async Task<List<Currency>> GetRatesAsync()
        {
            var result = new List<Currency>();
            try
            {
                string xml = await _httpClient.GetStringAsync(Url);
                XDocument doc = XDocument.Parse(xml);
                XNamespace ns = "http://www.bnr.ro/xsd";

                // Iterate over all <Rate> elements within the namespace
                foreach (var element in doc.Descendants(ns + "Rate"))
                {
                    string code = element.Attribute("currency")?.Value;
                    if (string.IsNullOrEmpty(code)) continue;

                    decimal rawValue = Convert.ToDecimal(element.Value, System.Globalization.CultureInfo.InvariantCulture);

                    // Handle the nominal multiplier (e.g., for HUF, JPY or KRW)
                    decimal multiplier = 1m;
                    var multiplierAttr = element.Attribute("multiplier");
                    if (multiplierAttr != null)
                    {
                        multiplier = Convert.ToDecimal(multiplierAttr.Value, System.Globalization.CultureInfo.InvariantCulture);
                    }

                    result.Add(new Currency
                    {
                        Code = code,
                        Name = code, // BNR XML omits full currency names
                        Value = rawValue / multiplier, // Neutralizing the multiplier trap
                        Source = "BNR (Romania)"
                    });
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[BNR Provider Error]: {ex.Message}");
            }
            return result;
        }
    }
}