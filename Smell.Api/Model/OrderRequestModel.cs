using Smell.Lib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace Smell.Api.Model
{
    public class OrderRequestModel
    {
        [EnumDataType(typeof(OrderType))]
        [Display(Name=nameof(OrderType))]
        [Required]
        public OrderType Type { get; set; }
        
        [DataType(DataType.Currency)]
        [Required]
        [Range(0.00000001, double.MaxValue)]
        public decimal Amount { get; set; }
    }
}
