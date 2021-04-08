using System;
using System.Collections.Generic;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace Wallet.Data.Models
{
    public partial class Transfers
    {
        public int Id { get; set; }
        public int OriginTransactionId { get; set; }
        public int DestinationTransactionId { get; set; }

        public virtual Transactions DestinationTransaction { get; set; }
        public virtual Transactions OriginTransaction { get; set; }
    }
}
