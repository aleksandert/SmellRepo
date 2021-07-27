using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Smell.Api.Model
{
    public class CryptoExchangeModel
    {
        [Display(Name="Ime")]
        public string Name { get; set; }
        public int Asks { get; set; }

        public int Bids { get; set; }

        public decimal BalanceBTC { get; set; }

        public decimal BalanceEUR { get; set; }
    }
}
