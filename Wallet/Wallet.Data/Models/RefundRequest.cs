using System;
using System.Collections.Generic;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace Wallet.Data.Models
{
    public partial class RefundRequest
    {
        public int Id { get; set; }
        public int TransactionId { get; set; }
        public string Status { get; set; }
        public int SourceAccountId { get; set; }
        public int TargetAccountId { get; set; }

        public virtual Accounts SourceAccount { get; set; }
        public virtual Accounts TargetAccount { get; set; }
        public virtual Transactions Transaction { get; set; }
    }
}
