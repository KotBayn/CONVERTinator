using CONVERTinator.Domain;
using CONVERTinator.Domain.GEO;
using CONVERTinator.Helpers;
using CONVERTinator.Services;
using CONVERTinator.Services.GeoLocator;
using CONVERTinator.Services.Regions.AmericasN.Facades;
using CONVERTinator.Services.Regions.Asia.Facades;
using CONVERTinator.Services.Regions.Asia.Providers; // for time, dont touch
using CONVERTinator.Services.Regions.CIS.Facades;
using CONVERTinator.Services.Regions.Europe.Facades;
using CONVERTinator.Services.Regions.Oceania.Facades;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CONVERTinator
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Console.Title = "CONVERTinator";

            Console.ForegroundColor = ConsoleColor.Cyan;
            string asciiArt = @"
╔═══════════════════════════════════════════════════════════════════════════════════════════════╗
║██████ ██████ ██   ██ ██   ██ ██████ █████  ██████ ██████ ██   ██   ███   ██████ ██████ █████  ║
║██     ██  ██ ███  ██ ██   ██ ██     ██  ██   ██     ██   ███  ██  ██ ██    ██   ██  ██ ██  ██ ║
║██     ██  ██ ██ █ ██ ██   ██ ██     █████    ██     ██   ██ █ ██ ███████   ██   ██  ██ █████  ║
║██     ██  ██ ██ █ ██  ██ ██  █████  ██  ██   ██     ██   ██ █ ██ ███████   ██   ██  ██ ██  ██ ║
║██     ██  ██ ██  ███  ██ ██  ██     ██  ██   ██     ██   ██  ███ ██   ██   ██   ██  ██ ██  ██ ║
║██████ ██████ ██   ██    █    ██████ ██  ██   ██   ██████ ██   ██ ██   ██   ██   ██████ ██  ██ ║
╚═══════════════════════════════════════════════════════════════════════════════════════════════╝
";
            Console.WriteLine(asciiArt);
            Console.ResetColor();

            // location detection
            Console.WriteLine("Initializing geo-location services...");
            ILocationService locationService = new LocationService();
            string currentIso = await locationService.GetCurrentCountryIsoCodeAsync();
            Country currentCountry = CountryRepository.GetCountryByIso(currentIso);

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"[Location Locked]: {currentCountry.IsoCode} (Base Region: {currentCountry.CountryRegion})\n");
            Console.ResetColor();

            // Menu
            Console.WriteLine("Select operation mode:");
            Console.WriteLine("[1] TRAVEL (Auto-location & Bordering countries)");
            Console.WriteLine("[2] BUSINESS (Global analytics & Regional rates)");
            Console.WriteLine("[0] Exit");
            Console.Write("\nYour choice > ");

            string? modeInput = Console.ReadLine()?.Trim();
            Console.Clear();

            string baseCurrency = "USD";
            var activeCurrencies = new List<string>();
            
            

            var dbRepository = new CONVERTinator.Repositories.DbRepository();
            var userSettings = await dbRepository.GetSettingsAsync();
            string currentCurrency = userSettings.BaseCurrency;

            switch (modeInput)
            {
                case "1":
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("=== Mode: TRAVEL ===");
                    Console.ResetColor();
                    currentCurrency = currentCountry.CurrencyCode;
                    baseCurrency = userSettings.BaseCurrency;
                    activeCurrencies = CountryRepository.GetTravelCurrencies(currentIso);
                    Console.WriteLine($"[Zone Loaded]: {activeCurrencies.Count} local & bordering currencies.\n");
                    break;

                case "2":
                    Console.ForegroundColor = ConsoleColor.Blue;
                    Console.WriteLine("=== Mode: BUSINESS ===");
                    Console.ResetColor();
                    baseCurrency = userSettings.BaseCurrency;
                    activeCurrencies = userSettings.SavedCurrencies
                        .Split(',', StringSplitOptions.RemoveEmptyEntries)
                        .ToList();

                    if (!activeCurrencies.Contains(baseCurrency)) activeCurrencies.Add(baseCurrency);
                    Console.WriteLine($"[Currencies Loaded]: {activeCurrencies.Count} mapped via JSON.\n");
                    break;

                case "0":
                    return;

                default:
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Error: Invalid mode selected.");
                    Console.ResetColor();
                    return;
            }

            // Zone & Provider Initialization
            List<Currency> allRates = new List<Currency>();

            TimeSpan cacheLifetime = TimeSpan.FromHours(2); // Time threshold for cache validity
            bool useCache = false;

            try
            { 
                useCache = await dbRepository.IsCacheFreshAsync(cacheLifetime);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[DB Warning] Integrity check failed: {ex.Message}. Forcing network fetch.");
            }

            if (useCache)
            {
                Console.ForegroundColor = ConsoleColor.DarkGreen;
                Console.WriteLine("[Cache Engine]: Fresh local rates detected. Skipping network overhead.");
                Console.ResetColor();

                allRates = await dbRepository.GetCachedRatesAsync();
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.DarkYellow;
                Console.WriteLine("[Cache Engine]: Cache is stale or missing. Initiating global network synchronization...");
                Console.ResetColor();
                Console.WriteLine("Calculating geopolitical network topology...");

                HashSet<Region> activeZones = CountryRepository.GetRequiredRegions(currentIso);

                Console.WriteLine($"Fetching provider data asynchronously for {activeZones.Count} region(s)...\n");

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

                // Data Fetching & Aggregation
                allRates = await globalComposite.GetRatesAsync();

                // --- Cache in DB ---
                if (allRates.Any())
                {
                    try
                    {
                        await dbRepository.SaveRatesAsync(allRates);
                        Console.WriteLine("[DB Success] Network data pushed to local cache storage.");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"[DB Error] Failed to write cache: {ex.Message}");
                    }
                }
            }

            Console.WriteLine($"Initialization complete. {allRates.Count} pairs aggregated.");
                Console.WriteLine("-----------------------------------------------------");
                Console.WriteLine("Commands: add [code], rem [code], ch [code], ex [amount], clear, exit");

                // Main Interaction Loop
                while (true)
                {
                    Console.ForegroundColor = ConsoleColor.Cyan;
                    Console.WriteLine($"\n[Base: {baseCurrency}] | [Active pairs: {activeCurrencies.Count}]");
                    Console.ResetColor();
                    Console.Write("> ");

                    string input = Console.ReadLine()?.Trim();
                    if (string.IsNullOrWhiteSpace(input)) continue;

                    string[] parts = input.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                    string command = parts[0].ToLower();
                    string arg = parts.Length > 1 ? parts[1].ToUpper() : "";

                    if (command == "exit") break;

                    switch (command)
                    {
                        case "clear":
                            Console.Clear();
                        break;

                        case "add":
                        if (string.IsNullOrEmpty(arg)) break;

                        if (!activeCurrencies.Contains(arg))
                        {
                            activeCurrencies.Add(arg);
                            Console.WriteLine($"[+] {arg} added.");

                            await dbRepository.SaveSettingsAsync(baseCurrency, activeCurrencies);
                        }
                        break;

                        case "rem":
                        if (activeCurrencies.Remove(arg))
                        {
                            Console.WriteLine($"[-] {arg} removed.");

                            await dbRepository.SaveSettingsAsync(baseCurrency, activeCurrencies);
                        }
                        break;

                    case "ch":
                        
                        if (!string.IsNullOrEmpty(arg) && arg.Length == 3 && arg.All(char.IsLetter))
                        {
                            baseCurrency = arg.ToUpper();
                            Console.WriteLine($"Base currency updated -> {baseCurrency}");

                            await dbRepository.SaveSettingsAsync(baseCurrency, activeCurrencies);
                        }
                        else
                        {
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.WriteLine("Error: Invalid currency code. Use 3 letters (e.g., USD, EUR).");
                            Console.ResetColor();
                        }
                    break;

                    case "ex":
                            if (!decimal.TryParse(arg, out decimal amount))
                            {
                                Console.WriteLine("Error: Invalid numeric format.");
                                break;
                            }

                            Console.WriteLine($"\n--- Conversion: {amount} {baseCurrency} ---");

                            foreach (var targetCurrency in activeCurrencies)
                            {
                                // Utilizing MedianCalculator to handle all cross-rate math
                                decimal? convertedAmount = MedianCalculator.Convert(amount, baseCurrency, targetCurrency, allRates);

                                if (convertedAmount == null)
                                {
                                    Console.ForegroundColor = ConsoleColor.DarkGray;
                                    Console.WriteLine($"{targetCurrency}: N/A (Rate missing)");
                                    Console.ResetColor();
                                }
                                else
                                {
                                    Console.ForegroundColor = ConsoleColor.Green;
                                    Console.WriteLine($"{targetCurrency}: {Math.Round(convertedAmount.Value, 3)}");
                                    Console.ResetColor();
                                }
                            }
                    break;

                        default:
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.WriteLine("Unknown command.");
                            Console.ResetColor();
                        break;
                    }
                }
        }
    }
}