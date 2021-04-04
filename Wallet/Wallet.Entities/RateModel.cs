using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Text.Json.Serialization;

namespace Wallet.Entities
{
    public class RateModel
    {
        public DateTime Date { get; set; }
        public double SellingPrice { get; set; }
        public double BuyingPrice { get; set; }
    }
}
