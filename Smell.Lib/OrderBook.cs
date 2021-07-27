using System;
using System.Collections.Generic;
using System.Linq;

namespace Smell.Lib
{
    public class OrderBook
    {
        public DateTime AcqTime { get; set; }
        public IEnumerable<OrderWrapper> Bids { get; set; }
        public IEnumerable<OrderWrapper> Asks { get; set; }
    }
}
