using CONVERTinator.Helpers;
using CONVERTinator.Services;
using CONVERTinator.Domain;
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
            Console.WriteLine("=== Starting CONVERTinator ===");

            // Журналисты - наши источники данных
            var journalists = new List<IExchangeRateProvider>
            {
                new CbrProvider(),
                new UsProvider(),
                new ChinaProvider(),
                new EcbXmlProvider()
            };

            var allRates = new List<Currency>();

            Console.WriteLine("Getting data from all sources. Please wait...\n");
            foreach (var journalist in journalists)
            {
                var rates = await journalist.GetRatesAsync();
                allRates.AddRange(rates);
                Console.WriteLine($"[+] {journalist.GetType().Name} brought {rates.Count} currencies.");
            }

            Console.WriteLine("\nData collection complete!");
            Console.WriteLine("-----------------------------------------------------");

            // Defoult
            string baseCurrency = "USD";
            var activeCurrencies = new List<string> { "EUR", "USD" };

            // Instructions
            Console.WriteLine("Available commands:");
            Console.WriteLine("  add [code]  - add a currency to the list (e.g., add BYN)");
            Console.WriteLine("  rem [code]  - remove a currency from the list (e.g., rem EUR)");
            Console.WriteLine("  ch [code]   - change the base currency (e.g., ch RUB)");
            Console.WriteLine("  ex [amount] - convert the base currency amount to all in the list (e.g., ex 100)");
            Console.WriteLine("  exit        - exit the program");

            while (true)
            {
                // Display the current state
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine($"\n[Base: {baseCurrency}] | [In list: {string.Join(", ", activeCurrencies)}]");
                Console.ResetColor();
                Console.Write("> ");
                string input = Console.ReadLine()?.Trim();

                if (string.IsNullOrWhiteSpace(input)) continue;

                // Split the input into parts by space.
                string[] parts = input.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                string command = parts[0].ToLower();

                // If there is no argument, leave it as an empty string.
                string arg = parts.Length > 1 ? parts[1].ToUpper() : "";

                if (command == "exit") break;

                // Command handling
                switch (command)
                {
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
                        // Try to convert the argument to a number (decimal)
                        if (!decimal.TryParse(arg, out decimal amount))
                        {
                            Console.WriteLine("Error: Enter a valid amount. Example: ex 150.50");
                            break;
                        }

                        Console.WriteLine($"\n--- Conversion {amount} {baseCurrency} ---");
                        // Iterate through all currencies in our active list
                        foreach (var targetCurrency in activeCurrencies)
                        {
                            // If the target currency matches the base currency, no conversion is needed
                            if (targetCurrency == baseCurrency)
                            {
                                Console.WriteLine($"{targetCurrency}: {amount}");
                                continue;
                            }

                            // Search for rates for the target currency in our large list   
                            var foundRates = allRates.Where(c => c.Code == targetCurrency).Select(c => c.Value).ToList();

                            if (foundRates.Count == 0)
                            {
                                Console.ForegroundColor = ConsoleColor.Yellow;
                                Console.WriteLine($"{targetCurrency}: Rate not found in any source.");
                                Console.ResetColor();
                                continue;
                            }

                            // Calculate the median
                            decimal medianRate = MedianCalculator.Calculate(foundRates);
                            decimal convertedAmount = amount * medianRate;

                            Console.ForegroundColor = ConsoleColor.Green;
                            Console.WriteLine($"{targetCurrency}: {Math.Round(convertedAmount, 3)}");
                            Console.ResetColor();
                        }
                        break;

                    default:
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("Unknown command. Available commands: add, rem, ch, ex, exit.");
                        Console.ResetColor();
                        break;
                }
            }
        }
    }
}