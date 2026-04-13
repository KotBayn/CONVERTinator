using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Collections.Generic;


namespace CONVERTinator
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("Привет, я КОНВЕРТинатор! я буду конвертировать нужные тебе валюты!");
            Console.WriteLine("------------Проверка подключения...------------");

            using (HttpClient client = new HttpClient())
            {
                //Обманка
                client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64)");


                var sources = new Dictionary<string, string>()
            {
                //цели
                {"RU (CBR Mirror)", "https://www.cbr-xml-daily.ru/daily_json.js" },
                {"EU (ECB)", "https://api.frankfurter.app/latest" },
                {"US (Global)", "https://api.exchangerate-api.com/v4/latest/USD" }
            };

                foreach (var source in sources)
                {
                    string name = source.Key;
                    string url = source.Value;

                    Console.WriteLine($"----------------------------------------------------");
                    Console.WriteLine($"Стучимся в: {name}");
                    Console.WriteLine($"URL: {url}");

                    try
                    {
                        //Скачиваем данные
                        client.Timeout = TimeSpan.FromSeconds(5);

                        string json = await client.GetStringAsync(url);

                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine("+");
                        Console.WriteLine($"Ответ: {json.Substring(0, 256)}...");
                        Console.ResetColor();
                    }

                    catch (Exception ex)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine($"ПРОВАЛ. Ошибка: {ex.Message}");
                        Console.ResetColor();
                    }
                    Console.WriteLine();
                }
            }
            Console.WriteLine("проверка завершена");
            Console.ReadLine();
        }
    }
}