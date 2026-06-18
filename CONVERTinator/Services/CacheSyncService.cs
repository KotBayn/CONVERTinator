using CONVERTinator.Domain;
using CONVERTinator.Domain.GEO;
using CONVERTinator.Helpers;
using CONVERTinator.Repositories;
using CONVERTinator.Services.GeoLocator;
using CONVERTinator.Services.Regions.AmericasN.Facades;
using CONVERTinator.Services.Regions.Asia.Facades;
using CONVERTinator.Services.Regions.Asia.Providers;
using CONVERTinator.Services.Regions.CIS.Facades;
using CONVERTinator.Services.Regions.Europe.Facades;
using CONVERTinator.Services.Regions.Oceania.Facades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CONVERTinator.Services
{
    public class CacheSyncService
    {
        public async Task ForceUpdateAsync()
        {
            var dbRepository = new DbRepository();
            var locationService = new LocationService();

            // Check GEO for server location
            string currentIso = await locationService.GetCurrentCountryIsoCodeAsync();
            HashSet<Region> activeZones = CountryRepository.GetRequiredRegions(currentIso);

            var globalComposite = new RegionProviderComposite("Global");

            if (activeZones.Contains(Region.CIS))
            {
                var cisComposite = new RegionProviderComposite("CIS");
                cisComposite.Add(new RussiaBanksFacade());
                cisComposite.Add(new MoldovaBanksFacade());
                cisComposite.Add(new BelarusBanksFacade());
                cisComposite.Add(new KazakhstanBanksFacade());
                globalComposite.Add(cisComposite);
            }

            if (activeZones.Contains(Region.Americas))
            {
                var americasComposite = new RegionProviderComposite("AmericasN");
                americasComposite.Add(new NorthAmericaBanksFacade());
                globalComposite.Add(americasComposite);
            }

            if (activeZones.Contains(Region.Asia))
            {
                var asiaComposite = new RegionProviderComposite("Asia");
                asiaComposite.Add(new ChinaProvider());
                asiaComposite.Add(new JapanBanksFacade());
                asiaComposite.Add(new IndiaBanksFacade());
                asiaComposite.Add(new SouthKoreaBanksFacade());
                asiaComposite.Add(new SingaporeBanksFacade());
                globalComposite.Add(asiaComposite);
            }

            if (activeZones.Contains(Region.Oceania))
            {
                var oceaniaComposite = new RegionProviderComposite("Oceania");
                oceaniaComposite.Add(new AustraliaBanksFacade());
                oceaniaComposite.Add(new NewZealandBanksFacade());
                globalComposite.Add(oceaniaComposite);
            }

            /*if (activeZones.Contains(Region.Africa))
            {
                var africaComposite = new RegionProviderComposite("Africa");
                africaComposite.Add(new SouthAfricaBanksFacade());
                globalComposite.Add(africaComposite);
            }*/

            /*if (activeZones.Contains(Region.MiddleEast))
            {
                var middleEastComposite = new RegionProviderComposite("Middle East");
                middleEastComposite.Add(new UAEProvider());
                middleEastComposite.Add(new SaudiArabiaBanksFacade());
                globalComposite.Add(middleEastComposite);
            }*/

            if (activeZones.Contains(Region.Europe))
            {
                var europeComposite = new RegionProviderComposite("Europe");
                europeComposite.Add(new GermanyBanksFacade());
                europeComposite.Add(new PolandBanksFacade());
                europeComposite.Add(new UkraineBanksFacade());
                europeComposite.Add(new BulgariaBanksFacade());
                europeComposite.Add(new ItalyBanksFacade());
                europeComposite.Add(new CzechBanksFacade());
                europeComposite.Add(new FranceBanksFacade());
                europeComposite.Add(new SpainBanksFacade());
                europeComposite.Add(new NetherlandsBanksFacade());
                europeComposite.Add(new HungaryBanksFacade());
                europeComposite.Add(new SwitzerlandBanksFacade());
                europeComposite.Add(new FinlandBanksFacade());
                europeComposite.Add(new SwedenBanksFacade());
                europeComposite.Add(new NorwayBanksFacade());
                europeComposite.Add(new DenmarkBanksFacade());
                europeComposite.Add(new PortugalBanksFacade());
                europeComposite.Add(new GreeceBanksFacade());
                europeComposite.Add(new GBBanksFacade());
                europeComposite.Add(new RomaniaBanksFacade());
                globalComposite.Add(europeComposite);
            }

            var allRates = await globalComposite.GetRatesAsync();

            if (allRates.Any())
            {
                await dbRepository.SaveRatesAsync(allRates);
            }
        }
    }
}