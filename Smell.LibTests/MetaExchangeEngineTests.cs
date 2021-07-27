using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using NUnit.Framework;
using Smell.Lib;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Smell.Lib.Tests
{
    [TestFixture()]
    public class MetaExchangeEngineTests
    {
        private ILogger<MetaExchangeEngine> logger;
        private string jsonPath;

        [SetUp]
        public void Init()
        {
            logger = new NullLogger<MetaExchangeEngine>();
            jsonPath = Path.Combine(AppContext.BaseDirectory, "Data\\order_books_data");
        }

        /// <summary>
        ///     Helper method for test data.
        /// </summary>
        /// <param name="balance"></param>
        /// <returns></returns>
        private IEnumerable<CryptoExchange> GetTestCryptoExchangeData(Balance balance)
        {
            var bids = new List<Order>() {
                new Order() { Amount = 0.1m, Type = OrderType.Buy, Price = 2500 },
                new Order() { Amount = 0.2m, Type = OrderType.Buy, Price = 2501 },
                new Order() { Amount = 0.4m, Type = OrderType.Buy, Price = 2505 },
                new Order() { Amount = 0.1m, Type = OrderType.Buy, Price = 2508 },
                new Order() { Amount = 0.1m, Type = OrderType.Buy, Price = 2510 },
                new Order() { Amount = 0.1m, Type = OrderType.Buy, Price = 2510 },
                new Order() { Amount = 0.2m, Type = OrderType.Buy, Price = 2511 },
                new Order() { Amount = 0.4m, Type = OrderType.Buy, Price = 2515 },
                new Order() { Amount = 0.1m, Type = OrderType.Buy, Price = 2518 },
                new Order() { Amount = 0.1m, Type = OrderType.Buy, Price = 2520 },
            };

            var asks = new List<Order>() {
                new Order() { Amount = 0.1m, Type = OrderType.Sell, Price = 2530 },
                new Order() { Amount = 0.2m, Type = OrderType.Sell, Price = 2521 },
                new Order() { Amount = 0.4m, Type = OrderType.Sell, Price = 2525 },
                new Order() { Amount = 0.1m, Type = OrderType.Sell, Price = 2528 },
                new Order() { Amount = 0.1m, Type = OrderType.Sell, Price = 2530 },
                new Order() { Amount = 0.1m, Type = OrderType.Sell, Price = 2530 },
                new Order() { Amount = 0.2m, Type = OrderType.Sell, Price = 2522 },
                new Order() { Amount = 0.4m, Type = OrderType.Sell, Price = 2525 },
                new Order() { Amount = 0.1m, Type = OrderType.Sell, Price = 2528 },
                new Order() { Amount = 0.1m, Type = OrderType.Sell, Price = 2530 },
            };

            return new List<CryptoExchange>()
            {
                new CryptoExchange()
                {
                    Name = "Test",
                    Balance = balance,
                    OrderBook = new OrderBook() {
                        AcqTime = DateTime.Now,
                        Asks = asks.Select(a=>new OrderWrapper() { Order = a }),
                        Bids = bids.Select(a=>new OrderWrapper() { Order = a }),
                    }
                }
            };
        }

        [Test()]
        public void ProcessOrder_NoBuyBalance()
        {
            var exchanges = CryptoExchangeLoader.LoadFromJsonFile(jsonPath, (x) => Balance.Zero, skip: 0, take: 1);
            var engine = new MetaExchangeEngine(exchanges, this.logger);

            var request = OrderRequest.BuyOrderRequest(5);
            var result = engine.ProcessOrder(request);

            Assert.Zero(result.TotalAmount);
        }

        [Test()]
        public void ProcessOrder_NoSellBalance()
        {
            var exchanges = CryptoExchangeLoader.LoadFromJsonFile(jsonPath, (x) => Balance.Zero, skip: 0, take: 1);
            var engine = new MetaExchangeEngine(exchanges, this.logger);

            var request = OrderRequest.BuyOrderRequest(5);
            var result = engine.ProcessOrder(request);

            Assert.Zero(result.TotalAmount);
        }

        [Test()]
        public void ProcessOrder_NotEnoughBuyBalance()
        {
            var exchanges = CryptoExchangeLoader.LoadFromJsonFile(jsonPath, (x) => Balance.FromEUR(10000), skip: 0, take: 1);
            var engine = new MetaExchangeEngine(exchanges, this.logger);

            var request = OrderRequest.BuyOrderRequest(5);
            var result = engine.ProcessOrder(request);

            Assert.Less(result.TotalAmount, request.Amount);
        }

        [Test()]
        public void ProcessOrder_NotEnoughSellBalance()
        {
            var exchanges = CryptoExchangeLoader.LoadFromJsonFile(jsonPath, (x) => Balance.FromBTC(4), skip: 0, take: 1);
            var engine = new MetaExchangeEngine(exchanges, this.logger);

            var request = OrderRequest.SellOrderRequest(5);
            var result = engine.ProcessOrder(request);

            Assert.Less(result.TotalAmount, request.Amount);
        }


        [Test()]
        //[TestCase(skip, take, amount, totalPrice, nrOfTrades)]
        [TestCase(0, 1, 2, 5930.40895, 4)]
        [TestCase(1, 1, 5, 14834.68533781, 9)]
        public void ProcessOrder_ProcessBuyRequest_ExpectTrades(int skip, int take, decimal amount, decimal totalPrice, int nrOfTrades)
        {
            var exchanges = CryptoExchangeLoader.LoadFromJsonFile(jsonPath, (x) => Balance.FromEUR(20000), skip, take);
            var engine = new MetaExchangeEngine(exchanges, this.logger);

            var request = OrderRequest.BuyOrderRequest(amount);
            var result = engine.ProcessOrder(request);

            Assert.AreEqual(request.Amount, result.TotalAmount);
            Assert.AreEqual(totalPrice, result.TotalPrice);
            Assert.AreEqual(nrOfTrades, result.Trades.Count);
        }

        [Test()]
        //[TestCase(skip, take, amount, totalPrice, nrOfTrades)]
        [TestCase(0, 1, 3, 8876.93811538, 8)]
        [TestCase(1, 1, 5, 14795.03054481, 9)]
        public void ProcessOrder_ProcessSellRequest_ExpectTrades(int skip, int take, decimal amount, decimal totalPrice, int nrOfTrades)
        {
            var exchanges = CryptoExchangeLoader.LoadFromJsonFile(jsonPath, (x) => Balance.FromBTC(10), skip, take);
            var engine = new MetaExchangeEngine(exchanges, this.logger);

            var request = OrderRequest.SellOrderRequest(amount);
            var result = engine.ProcessOrder(request);

            Assert.AreEqual(request.Amount, result.TotalAmount);
            Assert.AreEqual(totalPrice, result.TotalPrice);
            Assert.AreEqual(nrOfTrades, result.Trades.Count);
        }

        [Test()]
        public void ProcessOrder_ProcessBuyRequest_ExpectOk()
        {
            var balance = new Balance() { BTC = 10, EUR = 10000 };
            var exchanges = GetTestCryptoExchangeData(balance);
            
            var engine = new MetaExchangeEngine(exchanges, this.logger);

            var request = OrderRequest.BuyOrderRequest(0.5m);
            var result = engine.ProcessOrder(request);

            Assert.AreEqual(result.Status, "ok");
            Assert.AreEqual(10 + 0.5m, balance.BTC);
            Assert.AreEqual(10000 - (0.2 * 2521 + 0.2 * 2522 + 0.1 * 2525), balance.EUR);
        }

        [Test()]
        public void ProcessOrder_ProcessSellRequest_ExpectOk()
        {
            var balance = new Balance() { BTC = 10, EUR = 10000 };
            var exchanges = GetTestCryptoExchangeData(balance);

            var engine = new MetaExchangeEngine(exchanges, this.logger);

            var request = OrderRequest.SellOrderRequest(0.5m);
            var result = engine.ProcessOrder(request);

            Assert.AreEqual(result.Status, "ok");
            Assert.AreEqual(10 - 0.5m, balance.BTC);
            Assert.AreEqual(10000 + (0.1 * 2520 + 0.1 * 2518 + 0.3 * 2515), balance.EUR);
        }
    }
}