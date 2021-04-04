using System;
// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace Wallet.Data.Models
{
    public partial class Rates
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public double SellingPrice { get; set; }
        public double BuyingPrice { get; set; }
    }
}
