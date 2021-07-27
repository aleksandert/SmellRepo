using System;
using System.Collections.Generic;
using System.Text;

namespace Smell.Lib
{
    public class Trade
    {
        public string Exchange { get; set; }
        public OrderType Type { get; set; }
        public decimal Amount { get; set; }
        public decimal Price { get; set; }
        public DateTime Created => DateTime.UtcNow;
    }
}
