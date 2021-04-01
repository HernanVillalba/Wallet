using System;
using System.Collections.Generic;

namespace Wallet.Entities
{
    public class TransactionDetailsModel
    {
        public int Id { get; set; }
        public double Amount { get; set; }
        public string Concept { get; set; }
        public DateTime Date { get; set; }
        public string Type { get; set; }
        public int AccountId { get; set; }
        public bool? Editable { get; set; }
        public List<TransactionLogModel> TransactionLog { get; set; }
    }
}
