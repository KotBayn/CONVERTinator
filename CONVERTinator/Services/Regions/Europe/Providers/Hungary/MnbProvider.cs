using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using System.Xml.Linq;
using CONVERTinator.Domain;

namespace CONVERTinator.Services.Regions.Europe.Providers.Hungary
{
    public class MnbProvider : IExchangeRateProvider
    {
        private readonly HttpClient _httpClient = new HttpClient();
        private const string Url = "https://www.mnb.hu/arfolyamok.asmx"; // Magyar Nemzeti Bank

        public async Task<List<Currency>> GetRatesAsync()
        {
            var result = new List<Currency>();
            try
            {
                string xml = await _httpClient.GetStringAsync(Url);
                XDocument doc = XDocument.Parse(xml);
                XNamespace ns = "http://www.mnb.hu/xsd";

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
                        Name = code, // MNB XML omits full currency names
                        Value = rawValue / multiplier, // Neutralizing the multiplier trap
                        Source = "MNB (Hungary)"
                    });
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[MNB Provider Error]: {ex.Message}");
            }
            return result;
        }
    }
}