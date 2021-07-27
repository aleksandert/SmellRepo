using System;
using System.Collections.Generic;
using System.Text;

namespace Smell.Lib
{
    public class OrderRequest
    {
        public OrderType Type { get; set; }
        public decimal Amount { get; set; }

        protected OrderRequest(OrderType type, decimal amount)
        {
            this.Type = type;
            this.Amount = amount;
        }

        public OrderRequest()
        { }

        public static OrderRequest BuyOrderRequest(decimal amount) {
            return new BuyOrderRequest(amount);
        }

        public static OrderRequest SellOrderRequest(decimal amount)
        {
            return new SellOrderRequest(amount);
        }

    }

    public class BuyOrderRequest : OrderRequest
    {
        public BuyOrderRequest(decimal amount)
            : base(OrderType.Buy, amount)
        {
        }
    }

    public class SellOrderRequest : OrderRequest
    {
        public SellOrderRequest(decimal amount)
            : base(OrderType.Sell, amount)
        {
        } 
    }
}
