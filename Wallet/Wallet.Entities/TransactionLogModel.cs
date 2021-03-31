using System;
using System.Collections.Generic;
using System.Text;

namespace Wallet.Entities
{
    public class TransactionLogModel
    {
        public int Id { get; set; }
        public int TransactionId { get; set; }
        public string NewValue { get; set; }
        public DateTime ModificationDate { get; set; }
    }
}
