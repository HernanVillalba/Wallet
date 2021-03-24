using System;
using System.Collections.Generic;
using System.Text;

namespace Wallet.Entities
{
    public class FixedTermDepositModel
    {
        public int Id { get; set; }
        public double Amount { get; set; }
        public DateTime CreationDate { get; set; }
        public DateTime? ClosingDate { get; set; }
        public int AccountId { get; set; }
    }
}
