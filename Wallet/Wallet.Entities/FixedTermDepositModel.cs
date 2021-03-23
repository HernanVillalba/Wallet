using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace Wallet.Entities
{
    public class FixedTermDepositModel
    {
        [Required]
        [Range(1, int.MaxValue)]
        public int AccountId { get; set; }
        [Required]
        [Range(double.Epsilon, double.MaxValue)]
        public float Amount { get; set; }
    }
}
