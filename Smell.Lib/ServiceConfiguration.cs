using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Smell.Lib
{
    public static class ServiceConfiguration
    {
        public static IServiceCollection AddSmell(this IServiceCollection services)
        {
            // Register factory method used to generate random balances
            services.AddSingleton<Func<string, Balance>>(provider =>
            {
                return (s) => {
                    return new Balance()
                    {
                        EUR = decimal.Round((decimal)new Random().NextDouble() * 10000, 8) + 2000m,
                        BTC = decimal.Round((decimal)new Random().NextDouble() * 5, 8) + 0.5m,
                    };
                };
            });

            // Register factory method returning CryptoExchange data from data file.
            services.AddSingleton<IEnumerable<CryptoExchange>>(provider =>
            {
                var options = provider.GetService<IOptions<JsonCryptoExchangeLoaderOptions>>().Value;
                var funGetExchangeBalance = provider.GetService<Func<string, Balance>>();
                return CryptoExchangeLoader.LoadFromJsonFile(options.FilePath, funGetExchangeBalance, options.SkipLines, options.TakeLines);
            });

            // and finally, the engine itself
            services.AddScoped<MetaExchangeEngine>();

            return services;
        }
    }
}
