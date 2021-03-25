using System;
using System.Collections.Generic;
using System.Text;

namespace Wallet.Entities
{
    public class TransferModel
    {
        public int AccountId { get; set; }
        public int Amount { get; set; }
        public string Currency { get; set; }
    }
}
