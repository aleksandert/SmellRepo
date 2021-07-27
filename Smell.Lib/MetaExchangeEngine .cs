using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Smell.Lib
{
    public class MetaExchangeEngine
    {
        private IEnumerable<CryptoExchange> _cryptoExchanges;
        private readonly ILogger<MetaExchangeEngine> _logger;

        private IEnumerable<AskOrder> AllAsks { get; set; }
        private IEnumerable<BidOrder> AllBids { get; set; }

        public MetaExchangeEngine(IEnumerable<CryptoExchange> cryptoExchanges, ILogger<MetaExchangeEngine> logger)
        {
            _logger = logger;
            _cryptoExchanges = cryptoExchanges.ToList();
            
            // Preprepare list of all asks and bids available over crypto exchanges
            this.AllAsks = _cryptoExchanges
                            .SelectMany(x => x.OrderBook.Asks.Select(a => new AskOrder() { Order = a.Order, Exchange = x }))
                            .OrderBy(x => x.Order.Price)
                            .ToList(); // Materialize enumerable so the list isn't sorted on every access

            this.AllBids = _cryptoExchanges
                            .SelectMany(x => x.OrderBook.Bids.Select(a => new BidOrder() { Order = a.Order, Exchange = x }))
                            .OrderByDescending(x => x.Order.Price)
                            .ToList(); // Materialize enumerable so the list isn't sorted on every access
        }

        public TradesResponse ProcessOrder(OrderRequest request)
        {
            if (request.Amount <= 0)
                throw new ArgumentOutOfRangeException(nameof(request.Amount), "Amount must be greater than zero.");

            switch (request.Type)
            {
                case OrderType.Buy:
                    {
                        var trades = BuildBuyTrades(request.Amount);
                        return new TradesResponse(request, trades);
                    }
                case OrderType.Sell:
                    {
                        var trades = BuildSellTrades(request.Amount);
                        return new TradesResponse(request, trades);
                    }
            }
            throw new ArgumentOutOfRangeException(nameof(request.Type), "OrderType not recognized.");
        }

        /// <summary>
        /// Builds a "best price" list of trades.
        /// </summary>
        /// <param name="amount"></param>
        /// <returns>List of suggested trades</returns>
        private IEnumerable<Trade> BuildBuyTrades(decimal amount) 
        {
            var neededAmount = amount;
            
            // take orders until required amount is reached
            foreach (var ask in this.AllAsks)
            {
                var candidate = ask.Order;
                var balance = ask.Exchange.Balance;

                // ignore this exchange if balance is to small
                if (balance.EUR <= 0.1m)
                {
                    continue;
                }

                // take complete or only needed amount from candidate
                var tradeAmount = (neededAmount <= candidate.Amount) ? neededAmount : candidate.Amount;

                // check balance and buy less if to small
                if (balance.EUR < candidate.Price * tradeAmount)
                {
                    tradeAmount = decimal.Round(balance.EUR / candidate.Price, 8, MidpointRounding.ToZero);
                }

                // update running totals
                neededAmount -= tradeAmount;
                balance.EUR -= tradeAmount * candidate.Price;
                balance.BTC += tradeAmount;

                _logger.LogInformation($"{nameof(Trade)} created at {ask.Exchange.Name} for {tradeAmount} btc at {candidate.Price} eur/btc.");

                yield return new Trade()
                {
                    Exchange = ask.Exchange.Name,
                    Price = candidate.Price,
                    Amount = tradeAmount,
                    Type = OrderType.Buy,
                };
                
                if (neededAmount <= 0) {
                    yield break;
                }
            }
            _logger.LogWarning($"{nameof(BuildBuyTrades)} could not completely fullfill request (missing {neededAmount}/{amount} btc).");
        }

        /// <summary>
        /// Builds a "best offer" list of trades.
        /// </summary>
        /// <param name="amount"></param>
        /// <returns>List of suggested trades</returns>
        private IEnumerable<Trade> BuildSellTrades(decimal amount)
        {
            var neededAmount = amount;

            // take orders until required amount is reached
            foreach (var bid in this.AllBids)
            {
                var candidate = bid.Order;
                var balance = bid.Exchange.Balance;
                
                // ignore balances that are to small
                if (balance.BTC <= 0.0001m)
                {
                    continue;
                }

                // take full or needed amount from candidate
                var tradeAmount = (neededAmount <= candidate.Amount) ? neededAmount : candidate.Amount;

                // if we don't have enough balance, take as much as covered by balance
                if (balance.BTC < tradeAmount)
                {
                    tradeAmount = balance.BTC;
                }

                // update running totals
                neededAmount -= tradeAmount;
                balance.BTC -= tradeAmount;
                balance.EUR += tradeAmount * candidate.Price;

                _logger.LogInformation($"Sell {nameof(Trade)} created at {bid.Exchange.Name} for {tradeAmount} btc at {candidate.Price} eur/btc.");

                yield return new Trade()
                {
                    Exchange = bid.Exchange.Name,
                    Price = candidate.Price,
                    Amount = tradeAmount,
                    Type = OrderType.Sell,
                };

                // stop the loop if we're done
                if (neededAmount <= 0)
                {
                    yield break;
                }
            }
            _logger.LogWarning($"{nameof(BuildSellTrades)} could not completely fullfill request (missing {neededAmount}/{amount} btc).");
        }

        public IEnumerable<CryptoExchange> GetCryptoExchanges()
        {
            return _cryptoExchanges;
        }
    }
}
