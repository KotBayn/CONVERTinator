using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using System.Xml.Linq;
using CONVERTinator.Domain.Entities;
using CONVERTinator.Domain.Interfaces;

namespace CONVERTinator.Services.Regions.CIS.Providers.Moldova
{
    public class BnmProvider : IExchangeRateProvider
    {
        private readonly HttpClient _httpClient = new HttpClient();
        // official XML API of the National Bank of Moldova. Base - MDL.
        private const string Url = "https://bnm.md/ru/official_exchange_rates?get_xml=1";

        public async Task<List<Currency>> GetRatesAsync()
        {
            var result = new List<Currency>();
            try
            {
                string xml = await _httpClient.GetStringAsync(Url);
                XDocument doc = XDocument.Parse(xml);

                // Search <Valute>
                foreach (var element in doc.Descendants("Valute"))
                {
                    // Extract Nominal (e.g., 10 or 100)
                    decimal nominal = Convert.ToDecimal(element.Element("Nominal")?.Value, System.Globalization.CultureInfo.InvariantCulture);

                    // Extract Value. 
                    string rawValueStr = element.Element("Value")?.Value.Replace(',', '.');
                    decimal rawValue = Convert.ToDecimal(rawValueStr, System.Globalization.CultureInfo.InvariantCulture);

                    result.Add(new Currency
                    {
                        Code = element.Element("CharCode")?.Value,
                        Name = element.Element("Name")?.Value,
                        Value = rawValue / nominal,
                        Source = "BNM (Moldova)"
                    });
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[BNM Provider Error]: {ex.Message}");
            }
            return result;
        }
    }
}