using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CONVERTinator.Domain.GEO
{
    // Represents a country with its primary currency and bordering countries (zone)
        public class Country
        {
            public string IsoCode { get; set; }          // e.g., "PL", "DE"
            public string CurrencyCode { get; set; }     // e.g., "PLN", "EUR"
            public Region CountryRegion { get; set; }    // e.g., Region.Europe
            public List<string> Neighbors { get; set; }  // ISO codes of bordering countries
        }
}
