using CONVERTinator.Domain;
using CONVERTinator.Domain.GEO;
using CONVERTinator.Helpers;
using CONVERTinator.Repositories;
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
            var globalComposite = new RegionProviderComposite("Global Aggregator");
            Console.WriteLine("[API Cache Sync] Starting global network fetch for all world regions...");

            var cisComposite = new RegionProviderComposite("CIS");
                cisComposite.Add(new RussiaBanksFacade());
                cisComposite.Add(new MoldovaBanksFacade());
                cisComposite.Add(new BelarusBanksFacade());
                cisComposite.Add(new KazakhstanBanksFacade());
                globalComposite.Add(cisComposite);
            
            var americasComposite = new RegionProviderComposite("AmericasN");
                americasComposite.Add(new NorthAmericaBanksFacade());
                globalComposite.Add(americasComposite);
            
            var asiaComposite = new RegionProviderComposite("Asia");
                asiaComposite.Add(new ChinaProvider());
                asiaComposite.Add(new JapanBanksFacade());
                asiaComposite.Add(new IndiaBanksFacade());
                asiaComposite.Add(new SouthKoreaBanksFacade());
                asiaComposite.Add(new SingaporeBanksFacade());
                globalComposite.Add(asiaComposite);
            
            var oceaniaComposite = new RegionProviderComposite("Oceania");
                oceaniaComposite.Add(new AustraliaBanksFacade());
                oceaniaComposite.Add(new NewZealandBanksFacade());
                globalComposite.Add(oceaniaComposite);
            

          /*var africaComposite = new RegionProviderComposite("Africa");
                africaComposite.Add(new SouthAfricaBanksFacade());
                globalComposite.Add(africaComposite);*/

          /*var middleEastComposite = new RegionProviderComposite("Middle East");
                middleEastComposite.Add(new UAEProvider());
                middleEastComposite.Add(new SaudiArabiaBanksFacade());
                globalComposite.Add(middleEastComposite);*/

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
                
            var allRates = await globalComposite.GetRatesAsync();

            // All in SQLite DB
            if (allRates != null && allRates.Any())
            {
                await dbRepository.SaveRatesAsync(allRates);
                Console.WriteLine($"[API Cache Sync] Successfully cached {allRates.Count} currency pairs.");
            }
            else
            {
                throw new Exception("All banking providers failed to respond.");
            }
        }
    }
}