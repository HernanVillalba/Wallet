using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Wallet.API.Models
{
    public class AccountModel
    {
        public int Id { get; set; }
        public string Currency { get; set; }
        public double Balance { get; set; }
    }
}
