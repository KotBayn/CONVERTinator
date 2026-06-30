using CONVERTinator.Domain.Entities;
using CONVERTinator.Domain.GEO;
using CONVERTinator.Domain;
using CONVERTinator.Helpers;
using CONVERTinator.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CONVERTinator
{
    static class Program
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

            // GEO-Plug
            Console.WriteLine("Initializing mock geo-location for console admin mode...");
            string currentIso = "US"; 
            Country currentCountry = CountryRepository.GetCountryByIso(currentIso);

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"[Admin Location Locked]: {currentCountry.IsoCode} (Base Region: {currentCountry.CountryRegion})\n");
            Console.ResetColor();

            // Menu
            Console.WriteLine("Select operation mode:");
            Console.WriteLine("[1] TRAVEL (Auto-location & Bordering countries)");
            Console.WriteLine("[2] BUSINESS (Global analytics & Regional rates)");
            Console.WriteLine("[0] Exit");
            Console.Write("\nYour choice > ");

            string? modeInput = Console.ReadLine()?.Trim();
            Console.Clear();

            string baseCurrency = Constants.MainCurrency.USD;
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

            // Cache check and synchronization
            List<Currency> allRates = new List<Currency>();
            TimeSpan cacheLifetime = TimeSpan.FromHours(Constants.Cache.CacheExpirationHours);
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

                // Synchronize cache with network sources
                try
                {
                    var syncService = new CacheSyncService(dbRepository);
                    await syncService.ForceUpdateAsync();

                    allRates = await dbRepository.GetCachedRatesAsync();
                    Console.WriteLine("[DB Success] Global rates downloaded and cached.");
                }
                catch (Exception ex)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"[Error] {Constants.ErrorMessages.CacheUpdateFailed}: {ex.Message}");
                    Console.ResetColor();
                }
            }

            Console.WriteLine($"Initialization complete. {allRates.Count} pairs aggregated.");
            Console.WriteLine("-----------------------------------------------------");
            Console.WriteLine("Commands: add [code], rem [code], ch [code], ex [amount], clear, exit");

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
                            Console.WriteLine($"[Error]: {Constants.ErrorMessages.InvalidCurrencyCode}");
                            Console.ResetColor();
                        }
                        break;

                    case "ex":
                        if (!decimal.TryParse(arg, out decimal amount))
                        {
                            Console.WriteLine($"[Error]: {Constants.ErrorMessages.InvalidNumericFormat}");
                            break;
                        }

                        Console.WriteLine($"\n--- Conversion: {amount} {baseCurrency} ---");

                        foreach (var targetCurrency in activeCurrencies)
                        {
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
                        Console.WriteLine($"[Error]: {Constants.ErrorMessages.UnknownCommand}");
                        Console.ResetColor();
                        break;
                }
            }
        }
    }
}