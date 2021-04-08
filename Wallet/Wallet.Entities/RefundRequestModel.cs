using System;
using System.Collections.Generic;
using System.Text;

namespace Wallet.Entities
{
    public class RefundRequestModel
    {
        public int Id { get; set; }
        public int TransactionId { get; set; }
        public string Status { get; set; }
        public int SourceAccountId { get; set; }
        public int TargetAccountId { get; set; }
    }
}
