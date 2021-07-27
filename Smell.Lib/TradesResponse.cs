using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Smell.Lib
{
    public class TradesResponse
    {
        public OrderRequest Request { get; private set; }
        public IReadOnlyList<Trade> Trades { get; private set; }
        public decimal TotalPrice => Decimal.Round(Trades.Sum(x => x.Price * x.Amount), 8, MidpointRounding.AwayFromZero);
        public decimal TotalAmount => Decimal.Round(Trades.Sum(x => x.Amount), 8, MidpointRounding.AwayFromZero);
        public decimal ItemPrice => Decimal.Round( TotalPrice / TotalAmount, 8, MidpointRounding.AwayFromZero);
        public string Status => TotalAmount < Request.Amount ? $"NOT FULLFILLED ({TotalAmount}/{Request.Amount} in trades)" : "ok";

        public TradesResponse(OrderRequest request, IEnumerable<Trade> trades)
        {
            this.Request = request;
            this.Trades = new List<Trade>(trades).AsReadOnly();
        }
    }
}
