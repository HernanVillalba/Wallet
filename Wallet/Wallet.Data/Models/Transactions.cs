using System;
using System.Collections.Generic;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace Wallet.Data.Models
{
    public partial class Transactions
    {
        public Transactions()
        {
            RefundRequest = new HashSet<RefundRequest>();
            TransactionLog = new HashSet<TransactionLog>();
            TransfersDestinationTransaction = new HashSet<Transfers>();
            TransfersOriginTransaction = new HashSet<Transfers>();
        }

        public int Id { get; set; }
        public double Amount { get; set; }
        public string Concept { get; set; }
        public DateTime Date { get; set; }
        public string Type { get; set; }
        public int AccountId { get; set; }
        public int CategoryId { get; set; }

        public virtual Accounts Account { get; set; }
        public virtual Categories Category { get; set; }
        public virtual ICollection<RefundRequest> RefundRequest { get; set; }
        public virtual ICollection<TransactionLog> TransactionLog { get; set; }
        public virtual ICollection<Transfers> TransfersDestinationTransaction { get; set; }
        public virtual ICollection<Transfers> TransfersOriginTransaction { get; set; }
    }
}
