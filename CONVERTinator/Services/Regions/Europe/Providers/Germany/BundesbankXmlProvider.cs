using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using System.Xml.Linq;
using CONVERTinator.Domain.Entities;
using CONVERTinator.Domain.Interfaces;

namespace CONVERTinator.Services.Regions.Europe.Providers.Germany
{
    public class BundesbankXmlProvider : IExchangeRateProvider
    {
        private readonly HttpClient _httpClient = new HttpClient();
        private const string Url = "https://api.statdata.bundesbank.de/Rest/data/BBEX3/D.USD.EUR.SP00.A";

        public async Task<List<Currency>> GetRatesAsync()
        {
            var result = new List<Currency>();
            try
            {
                // Downloading XML, loading it into XDocument
                string xml = await _httpClient.GetStringAsync(Url);
                XDocument doc = XDocument.Parse(xml);

                // Looking for all <Cube> tags with a currency attribute
                // Ignoring xmlns (namespace) for simplicity, searching by local name   
                foreach (var element in doc.Descendants())
                {
                    if (element.Name.LocalName == "Cube" && element.Attribute("currency") != null)
                    {
                        result.Add(new Currency
                        {
                            Code = element.Attribute("currency").Value,
                            Name = element.Attribute("currency").Value,
                            Value = Convert.ToDecimal(element.Attribute("rate").Value, System.Globalization.CultureInfo.InvariantCulture),
                            Source = "Bundesbank (XML)"
                        });
                    }
                }
            }
            catch (Exception ex) { Console.WriteLine($"Error in Bundesbank XML Parser: {ex.Message}"); }
            return result;
        }
    }
}