using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using Smell.Lib;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Linq;

namespace Smell.ConsoleApp
{
    class Program
    {
        private static int skipLines;
        private static int takeLines;
        private static string filePath = "Data\\order_books_data";

        static void Main(string[] args)
        {
            if (args.Length == 2)
            {
                if (!int.TryParse(args[0], out skipLines))
                {
                    skipLines = 0;
                }

                if (!int.TryParse(args[1], out takeLines))
                {
                    takeLines = int.MaxValue;
                }
            }

            //IoC service provider
            ServiceProvider serviceProvider = BuildServiceProvider();

            var engine = serviceProvider.GetService<MetaExchangeEngine>();

            var states = engine.GetCryptoExchanges();

            OutputExchangeState(states);

            // Get inputs for request
            while (true)
            {
                var request = InputRequest();

                if (request != null)
                {
                    var response = engine.ProcessOrder(request);

                    OutputResponse(response);
                }

                OutputExchangeState(states);

                Console.WriteLine("ESC to exit, other to repeat.");

                if (Console.ReadKey().Key == ConsoleKey.Escape)
                {
                    break;
                }
            }

            OutputExchangeState(states);

            Console.WriteLine("Done.");
        }

        private static void OutputExchangeState(IEnumerable<CryptoExchange> states)
        {
            Console.WriteLine("Exchange states: ");

            foreach (var exc in states)
            {
                Console.WriteLine("\t'{0}' (EUR={1}, BTC={2})", exc.Name, exc.Balance.EUR, exc.Balance.BTC);
            }
        }

        private static void OutputResponse(TradesResponse response)
        {
            Console.WriteLine("\nRESPONSE: Status: {3} / TotalAmount={0}, TotalPrice={1}, Price={2}", response.TotalAmount, response.TotalPrice, response.ItemPrice, response.Status);
            foreach (var trade in response.Trades)
            {
                Console.WriteLine("\tOrder to {0}: {3} {1} BTC at {2} EUR", trade.Exchange, trade.Amount, trade.Price, trade.Type);
            }
        }

        private static OrderRequest InputRequest()
        {
            var request = new OrderRequest();

            while (true)
            {
                Console.WriteLine("\nSelect order type: (0=Buy, 1=Sell, ESC=Exit)\n");

                var orderType = Console.ReadKey();

                if (orderType.Key == ConsoleKey.Escape)
                {
                    return null;
                } 
                else if (orderType.KeyChar == '0')
                {
                    request.Type = OrderType.Buy;
                    break;
                }
                else if (orderType.KeyChar == '1')
                {
                    request.Type = OrderType.Sell;
                    break;
                }
                
                Console.WriteLine("Invalid selection... \n");
            }

            while (true)
            {
                Console.WriteLine("Amount?\n");

                var strAmount = Console.ReadLine();

                if (string.IsNullOrEmpty(strAmount))
                {
                    return null;
                }

                decimal amount;
                
                if (decimal.TryParse(strAmount, out amount))
                {
                    request.Amount = amount;
                    break;
                }
                Console.WriteLine("Invalid amount...\n");
            }
            return request;
        }


        private static ServiceProvider BuildServiceProvider()
        {
            var services = new ServiceCollection();

            services.AddLogging(loggerBuilder =>
            {
                loggerBuilder.ClearProviders();
                //loggerBuilder.AddConsole();
            });

            services.Configure<JsonCryptoExchangeLoaderOptions>(opt =>
            {
                opt.FilePath = Path.Combine(AppContext.BaseDirectory, filePath);
                opt.SkipLines = skipLines;
                opt.TakeLines = takeLines;
            });

            services.AddSmell();

            return services.BuildServiceProvider();
        }
    }
}
