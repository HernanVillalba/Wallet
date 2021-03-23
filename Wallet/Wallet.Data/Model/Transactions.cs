using Newtonsoft.Json;
using System;

namespace Wallet.Data.Models
{
    public partial class Transactions
    {
        public int Id { get; set; }
        public double Amount { get; set; }
        public string Concept { get; set; }
        public DateTime Date { get; set; }
        public string Type { get; set; }
        public int AccountId { get; set; }
        [JsonIgnore]
        public virtual Accounts Account { get; set; }
    }
}
