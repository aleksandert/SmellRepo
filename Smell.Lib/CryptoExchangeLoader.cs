﻿using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Smell.Lib
{
    /// <summary>
    /// Exposes static methods used to load data file.
    /// </summary>
    public class CryptoExchangeLoader 
    {
        public static IEnumerable<CryptoExchange> LoadFromJsonFile(string dataFile, int skip = 0, int take = int.MaxValue) {
            return LoadFromJsonFile(dataFile, (x) => Balance.Zero, skip, take);
        }

        public static IEnumerable<CryptoExchange> LoadFromJsonFile(string dataFile, Func<string, Balance> getExchangeBalance, int skip = 0, int take = int.MaxValue)
        {
            return File.ReadLines(dataFile)
                    .AsParallel()
                    .Skip(skip).Take(take)
                    .Select(line =>
                    {
                        int startPos = line.IndexOf('{');
                        return new
                        {
                            Name = line.Substring(0, startPos).TrimEnd(),
                            JsonData = line.Substring(startPos),
                        };
                    })
                    .Select(x => {
                        System.Diagnostics.Debug.WriteLine("Thread: {0}, Start: {1}", System.Threading.Thread.CurrentThread.ManagedThreadId, DateTime.UtcNow.Ticks);

                        var cryptoExchange = new CryptoExchange()
                        {
                            Name = x.Name,
                            OrderBook = JsonConvert.DeserializeObject<OrderBook>(x.JsonData),
                            Balance = getExchangeBalance(x.Name),
                        };

                        System.Diagnostics.Debug.WriteLine("Thread: {0}, End: {1}", System.Threading.Thread.CurrentThread.ManagedThreadId, DateTime.UtcNow.Ticks);

                        return cryptoExchange;
                    });
        }
    }

    public class JsonCryptoExchangeLoaderOptions
    {
        public const string SectionName = "CryptoExchange";
        public string FilePath { get; set; }
        public int SkipLines { get; set; }
        public int TakeLines { get; set; }
    }
}
