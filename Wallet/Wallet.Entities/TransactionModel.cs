using System;
using System.Collections.Generic;
using System.Text;

namespace Wallet.Entities
{
    public class TransactionModel
    {
        public int Id { get; set; }
        public double Amount { get; set; }
        public string Concept { get; set; }
        public DateTime Date { get; set; }
        public string Type { get; set; }
        public int AccountId { get; set; }
        public int CategoryId { get; set; }
    }
}
