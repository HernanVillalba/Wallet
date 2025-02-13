﻿using System;
using System.Collections.Generic;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace Wallet.Data.Models
{
    public partial class TransactionLog
    {
        public int Id { get; set; }
        public int TransactionId { get; set; }
        public string NewValue { get; set; }
        public DateTime ModificationDate { get; set; }

        public virtual Transactions Transaction { get; set; }
    }
}
