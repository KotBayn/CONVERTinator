using CONVERTinator.Helpers;
using CONVERTinator.Services;
using CONVERTinator.Domain;
using CONVERTinator.Services.GeoLocator; 
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CONVERTinator
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Console.Title = "CONVERTinator";

            // Menu
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("=================================================");
            Console.WriteLine("                  CONVERTinator                  ");
            Console.WriteLine("=================================================\n");
            Console.ResetColor();

            Console.WriteLine("Select operation mode:");
            Console.WriteLine("[1] TRAVEL (Auto-location & Bordering countries)");
            Console.WriteLine("[2] BUSINESS (Global analytics & Median rates)");
            Console.WriteLine("[0] Exit");
            Console.Write("\nYour choice > ");

            string modeInput = Console.ReadLine()?.Trim();
            Console.Clear(); 

            var journalists = new List<IExchangeRateProvider>();
            string baseCurrency = "USD";
            var activeCurrencies = new List<string> {};

            switch (modeInput)
            {
                case "1":
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("=== Mode: TRAVEL ===");
                    Console.ResetColor();

                    ILocationService locationService = new LocationService();
                    string currentIsoCode = await locationService.GetCurrentCountryIsoCodeAsync();
                    Console.WriteLine($"[Auto-Location detected]: {currentIsoCode}\n");

                    // TODO: make a real mapping of ISO codes to base currencies and neighboring countries.
                    baseCurrency = $"{currentIsoCode}";
                    activeCurrencies = new List<string> { "AFN", "ALL", "AMD", "ANG", "AOA", "ARS", "AUD", "AWG",
                        "AZN", "BAM", "BBD", "BDT", "BGN", "BHD", "BIF", "BMD", "BND", "BOB", "BRL", "BSD", "BTN",
                        "BWP", "BYN", "BZD", "CAD", "CDF", "CHF", "CLP", "CNY", "COP", "CRC", "CUC", "CUP", "CVE",
                        "CZK", "DJF", "DKK", "DOP", "DZD", "EGP", "ERN", "ETB", "EUR", "FJD", "FKP", "GBP", "GEL",
                        "GGP", "GHS", "GIP", "GMD", "GNF", "GTQ", "GYD", "HKD", "HNL", "HRK", "HTG", "HUF", "IDR",
                        "ILS", "IMP", "INR", "IQD", "IRR", "ISK", "JEP", "JMD", "JOD", "JPY", "KES", "KGS", "KHR",
                        "KMF", "KPW", "KRW", "KWD", "KYD", "KZT", "LAK", "LBP", "LKR", "LRD", "LSL", "LYD", "MAD",
                        "MDL", "MGA", "MKD", "MMK", "MNT", "MOP", "MRU", "MUR", "MVR", "MWK", "MXN", "MYR", "MZN",
                        "NAD", "NGN", "NIO", "NOK", "NPR", "NZD", "OMR", "PAB", "PEN", "PGK", "PHP", "PKR", "PLN",
                        "PYG", "QAR", "RON", "RSD", "RUB", "RWF", "SAR", "SBD", "SCR", "SDG", "SEK", "SGD", "SHP",
                        "SLL", "SOS", "SRD", "SSP", "STN", "SVC", "SYP", "SZL", "THB", "TJS", "TMT", "TND", "TOP",
                        "TRY", "TTD", "TVD", "TWD", "TZS", "UAH", "UGX", "USD", "UYU", "UZS", "VES", "VND", "VUV",
                        "WST", "XAF", "XCD", "XOF", "XPF", "YER", "ZAR", "ZMW", "ZWL" };

                    journalists.Add(new CbrProvider());
                    journalists.Add(new UsProvider());
                    journalists.Add(new ChinaProvider());
                    journalists.Add(new EcbXmlProvider());

                    break;

                case "2":
                    Console.ForegroundColor = ConsoleColor.Blue;
                    Console.WriteLine("=== Mode: BUSINESS ===");
                    Console.ResetColor();

                    baseCurrency = $"";
                    activeCurrencies = new List<string> { "AFN", "ALL", "AMD", "ANG", "AOA", "ARS", "AUD", "AWG",
                        "AZN", "BAM", "BBD", "BDT", "BGN", "BHD", "BIF", "BMD", "BND", "BOB", "BRL", "BSD", "BTN",
                        "BWP", "BYN", "BZD", "CAD", "CDF", "CHF", "CLP", "CNY", "COP", "CRC", "CUC", "CUP", "CVE",
                        "CZK", "DJF", "DKK", "DOP", "DZD", "EGP", "ERN", "ETB", "EUR", "FJD", "FKP", "GBP", "GEL",
                        "GGP", "GHS", "GIP", "GMD", "GNF", "GTQ", "GYD", "HKD", "HNL", "HRK", "HTG", "HUF", "IDR",
                        "ILS", "IMP", "INR", "IQD", "IRR", "ISK", "JEP", "JMD", "JOD", "JPY", "KES", "KGS", "KHR",
                        "KMF", "KPW", "KRW", "KWD", "KYD", "KZT", "LAK", "LBP", "LKR", "LRD", "LSL", "LYD", "MAD",
                        "MDL", "MGA", "MKD", "MMK", "MNT", "MOP", "MRU", "MUR", "MVR", "MWK", "MXN", "MYR", "MZN",
                        "NAD", "NGN", "NIO", "NOK", "NPR", "NZD", "OMR", "PAB", "PEN", "PGK", "PHP", "PKR", "PLN",
                        "PYG", "QAR", "RON", "RSD", "RUB", "RWF", "SAR", "SBD", "SCR", "SDG", "SEK", "SGD", "SHP",
                        "SLL", "SOS", "SRD", "SSP", "STN", "SVC", "SYP", "SZL", "THB", "TJS", "TMT", "TND", "TOP",
                        "TRY", "TTD", "TVD", "TWD", "TZS", "UAH", "UGX", "USD", "UYU", "UZS", "VES", "VND", "VUV",
                        "WST", "XAF", "XCD", "XOF", "XPF", "YER", "ZAR", "ZMW", "ZWL" };

                    // For buisness mode it's crucial to have as many sources as possible to calculate a reliable median rate.
                    journalists.Add(new CbrProvider());
                    journalists.Add(new UsProvider());
                    journalists.Add(new ChinaProvider());
                    journalists.Add(new EcbXmlProvider());
                    break;

                case "0":
                    return;

                default:
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Error: Invalid mode selected.");
                    Console.ResetColor();
                    return;
            }

            // PARALLEL DATA COLLECTION
            Console.WriteLine("Getting data from selected sources. Please wait...\n");
            var allRates = new List<Currency>();
            var fetchTasks = journalists.Select(j => j.GetRatesAsync());
            var results = await Task.WhenAll(fetchTasks);

            foreach (var rates in results)
            {
                allRates.AddRange(rates);
            }

            Console.WriteLine($"\nData collection complete! Fetched {allRates.Count} pairs.");
            Console.WriteLine("-----------------------------------------------------");

            // INTERACTIVE LOOP
            Console.WriteLine("Available commands:");
            Console.WriteLine("  add [code]  - add a currency to the list (e.g., add BYN)");
            Console.WriteLine("  rem [code]  - remove a currency from the list (e.g., rem EUR)");
            Console.WriteLine("  ch [code]   - change the base currency (e.g., ch RUB)");
            Console.WriteLine("  ex [amount] - convert the base currency amount to all in the list (e.g., ex 100)");
            Console.WriteLine("  clear       - clear the console screen to keep it beautiful");
            Console.WriteLine("  exit        - exit the program");

            while (true)
            {
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine($"\n[Base: {baseCurrency}]");
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
                        if (string.IsNullOrEmpty(arg))
                        {
                            Console.WriteLine("Error: Specify a currency code. Example: add BYN");
                            break;
                        }
                        if (!activeCurrencies.Contains(arg))
                        {
                            activeCurrencies.Add(arg);
                            Console.WriteLine($"Currency {arg} added to the list.");
                        }
                        else
                        {
                            Console.WriteLine($"Currency {arg} is already in the list.");
                        }
                        break;

                    case "rem":
                        if (activeCurrencies.Contains(arg))
                        {
                            activeCurrencies.Remove(arg);
                            Console.WriteLine($"Currency {arg} removed from the list.");
                        }
                        else
                        {
                            Console.WriteLine($"Error: Currency {arg} is not in the list.");
                        }
                        break;

                    case "ch":
                        if (string.IsNullOrEmpty(arg))
                        {
                            Console.WriteLine("Error: Specify a currency code. Example: ch BYN");
                            break;
                        }
                        baseCurrency = arg;
                        Console.WriteLine($"Base currency changed to {baseCurrency}.");
                        break;

                    case "ex":
                        if (!decimal.TryParse(arg, out decimal amount))
                        {
                            Console.WriteLine("Error: Enter a valid amount. Example: ex 150.50");
                            break;
                        }

                        Console.WriteLine($"\n--- Conversion {amount} {baseCurrency} ---");

                        foreach (var targetCurrency in activeCurrencies)
                        {
                            if (targetCurrency == baseCurrency)
                            {
                                Console.WriteLine($"{targetCurrency}: {amount}");
                                continue;
                            }

                            var foundRates = allRates.Where(c => c.Code == targetCurrency).Select(c => c.Value).ToList();

                            if (foundRates.Count == 0)
                            {
                                Console.ForegroundColor = ConsoleColor.Yellow;
                                Console.WriteLine($"{targetCurrency}: Rate not found in any source.");
                                Console.ResetColor();
                                continue;
                            }

                            decimal medianRate = MedianCalculator.Calculate(foundRates);
                            decimal convertedAmount = amount * medianRate;

                            Console.ForegroundColor = ConsoleColor.Green;
                            Console.WriteLine($"{targetCurrency}: {Math.Round(convertedAmount, 3)}");
                            Console.ResetColor();
                        }
                        break;

                    default:
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("Unknown command. Available commands: add, rem, ch, ex, clear, exit.");
                        Console.ResetColor();
                        break;
                }
            }
        }
    }
}