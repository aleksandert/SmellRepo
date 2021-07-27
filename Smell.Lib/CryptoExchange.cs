using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Smell.Lib
{
    public class CryptoExchange
    {
        public string Name { get; set; }

        public OrderBook OrderBook { get; set; }

        public Balance Balance { get; set; }
    }
}
