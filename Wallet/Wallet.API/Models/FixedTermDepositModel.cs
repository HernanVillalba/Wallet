using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace Wallet.API.Models
{
    public class FixedTermDepositModel
    {
        [Required]
        public int AccountId { get; set; }
        [Required]
        public float Amount { get; set; }
    }
}
