using System;
using System.Collections.Generic;
using System.Text;

namespace Smell.Lib
{
    public class Balance
    {
        private static Balance _zeroBalance = new Balance();
        public decimal EUR { get; set; }
        public decimal BTC { get; set; }
        public static Balance Zero => _zeroBalance;

        public static Balance FromEUR(decimal eur) {
            return new Balance() { EUR = eur };
        }

        public static Balance FromBTC(decimal btc)
        {
            return new Balance() { BTC = btc };
        }
    }
}
