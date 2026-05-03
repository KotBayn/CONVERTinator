using CONVERTinator.Domain;
using CONVERTinator.Domain.GEO;
using CONVERTinator.Helpers;
using CONVERTinator.Services;
using CONVERTinator.Services.GeoLocator;
using CONVERTinator.Services.RegionProvider;
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

            Console.WriteLine("Select operation mode:");
            Console.WriteLine("[1] TRAVEL (Auto-location & Bordering countries)");
            Console.WriteLine("[2] BUSINESS (Global analytics & Regional rates)");
            Console.WriteLine("[0] Exit");
            Console.Write("\nYour choice > ");

            string modeInput = Console.ReadLine()?.Trim();
            Console.Clear();

            string baseCurrency = "USD";
            var activeCurrencies = new List<string>();
            ILocationService locationService = new LocationService();

            // Configuration based on selected mode
            switch (modeInput)
            {
                case "1":
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("=== Mode: TRAVEL ===");
                    Console.ResetColor();

                    string travelIso = await locationService.GetCurrentCountryIsoCodeAsync();
                    Country travelCountry = CountryRepository.GetCountryByIso(travelIso);

                    baseCurrency = travelCountry.CurrencyCode;
                    activeCurrencies = CountryRepository.GetTravelCurrencies(travelIso);

                    Console.WriteLine($"[Auto-Location]: {travelIso}");
                    Console.WriteLine($"[Zone Loaded]: {activeCurrencies.Count} local & bordering currencies.\n");
                    break;

                case "2":
                    Console.ForegroundColor = ConsoleColor.Blue;
                    Console.WriteLine("=== Mode: BUSINESS ===");
                    Console.ResetColor();

                    string bizIso = await locationService.GetCurrentCountryIsoCodeAsync();
                    Country bizCountry = CountryRepository.GetCountryByIso(bizIso);

                    baseCurrency = "USD";
                    activeCurrencies = RegionRepository.GetCurrenciesByRegion(bizCountry.CountryRegion);

                    if (!activeCurrencies.Contains(baseCurrency))
                    {
                        activeCurrencies.Add(baseCurrency);
                    }

                    Console.WriteLine($"[Region Detected]: {bizCountry.CountryRegion}");
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

            // Setting up Composite Pattern for Data Providers
            Console.WriteLine("Fetching provider data asynchronously...\n");

            var globalComposite = new RegionProviderComposite("Global");

            var cisComposite = new RegionProviderComposite("CIS");
            cisComposite.Add(new CbrProvider());

            var americasComposite = new RegionProviderComposite("Americas");
            americasComposite.Add(new UsProvider());

            var asiaComposite = new RegionProviderComposite("Asia");
            asiaComposite.Add(new ChinaProvider());

            var europeComposite = new RegionProviderComposite("Europe");
            europeComposite.Add(new EcbXmlProvider());

            globalComposite.Add(cisComposite);
            globalComposite.Add(americasComposite);
            globalComposite.Add(asiaComposite);
            globalComposite.Add(europeComposite);

            // Fetch all rates in one line! The composite handles Task.WhenAll internally.
            List<Currency> allRates = await globalComposite.GetRatesAsync();

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
                        }
                        break;

                    case "rem":
                        if (activeCurrencies.Remove(arg))
                        {
                            Console.WriteLine($"[-] {arg} removed.");
                        }
                        break;

                    case "ch":
                        if (!string.IsNullOrEmpty(arg))
                        {
                            baseCurrency = arg;
                            Console.WriteLine($"Base currency updated -> {baseCurrency}");
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