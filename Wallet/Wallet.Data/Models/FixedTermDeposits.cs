using System;
using System.Collections.Generic;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace Wallet.Data.Models
{
    public partial class FixedTermDeposits
    {
        public int Id { get; set; }
        public double Amount { get; set; }
        public DateTime CreationDate { get; set; }
        public DateTime? ClosingDate { get; set; }
        public int AccountId { get; set; }

        public virtual Accounts Account { get; set; }
    }
}
