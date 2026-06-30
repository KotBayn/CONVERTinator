using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using System.Xml.Linq;
using CONVERTinator.Domain.Entities;
using CONVERTinator.Domain.Interfaces;

namespace CONVERTinator.Services.Regions.CIS.Providers.Kazakhstan
{
    public class NbrkProvider : IExchangeRateProvider
    {
        private readonly HttpClient _httpClient = new HttpClient();
        // National Bank of Kazakhstan XML RSS Feed
        private const string Url = "https://nationalbank.kz/rss/rates_all.xml";

        public async Task<List<Currency>> GetRatesAsync()
        {
            var result = new List<Currency>();
            try
            {
                string xml = await _httpClient.GetStringAsync(Url);
                XDocument doc = XDocument.Parse(xml);

                foreach (var element in doc.Descendants("item"))
                {
                    string code = element.Element("title")?.Value;
                    string name = element.Element("fullname")?.Value;

                    if (string.IsNullOrEmpty(code)) continue;

                    decimal quant = Convert.ToDecimal(element.Element("quant")?.Value ?? "1", System.Globalization.CultureInfo.InvariantCulture);
                    decimal descriptionRate = Convert.ToDecimal(element.Element("description")?.Value, System.Globalization.CultureInfo.InvariantCulture);

                    result.Add(new Currency
                    {
                        Code = code,
                        Name = name,
                        Value = descriptionRate / quant,
                        Source = "NBKZ (Kazakhstan)"
                    });
                }
            }
            catch (Exception ex) { Console.WriteLine($"[NBKZ Error]: {ex.Message}"); }
            return result;
        }
    }
}