using CONVERTinator.Domain.Entities;
using CONVERTinator.Domain.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CONVERTinator.Services
{
    public class RegionProviderComposite : IExchangeRateProvider
    {
        private readonly List<IExchangeRateProvider> _children = new List<IExchangeRateProvider>();
        public string RegionName { get; }

        public RegionProviderComposite(string regionName)
        {
            RegionName = regionName;
        }

        public void Add(IExchangeRateProvider component)
        {
            _children.Add(component);
        }

        public void Remove(IExchangeRateProvider component)
        {
            _children.Remove(component);
        }

        // Main method to get rates from all children
        public async Task<List<Currency>> GetRatesAsync()
        {
            var allRates = new List<Currency>();

            // parallel Query
            var tasks = _children.Select(child => child.GetRatesAsync());
            var results = await Task.WhenAll(tasks);

            foreach (var rates in results)
            {
                allRates.AddRange(rates);
            }

            return allRates;
        }
    }
}