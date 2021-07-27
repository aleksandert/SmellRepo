using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Smell.Lib
{
    public class Order
    {
        public object Id { get; set; }
        public DateTime Time { get; set; }
        public OrderType Type { get; set; }
        public string Kind { get; set; }
        public decimal Amount { get; set; }
        public decimal Price { get; set; }
    }

    public class AskOrder { 
        public Order Order { get; set; }
        public CryptoExchange Exchange { get; set; }
    }

    public class BidOrder
    {
        public Order Order { get; set; }
        public CryptoExchange Exchange { get; set; }
    }

    public class OrderWrapper {
        public Order Order { get; set; }
    }

    public enum OrderType { 
        Buy,
        Sell
    }
}
