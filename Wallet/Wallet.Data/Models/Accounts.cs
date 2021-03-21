using System;
using System.Collections.Generic;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace Wallet.Data.ModelsAPI
{
    public partial class Accounts
    {
        public Accounts()
        {
            FixedTermDeposit = new HashSet<FixedTermDeposit>();
            Transactions = new HashSet<Transactions>();
        }

        public int Id { get; set; }
        public string Currency { get; set; }
        public int UserId { get; set; }

        public virtual Users User { get; set; }
        public virtual ICollection<FixedTermDeposit> FixedTermDeposit { get; set; }
        public virtual ICollection<Transactions> Transactions { get; set; }
    }
}
