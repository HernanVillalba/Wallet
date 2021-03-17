using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Wallet.API.Models
{
    public class TransactionModel
    {
        [Required]
        public float Amount { get; set; }
        [Required]
        public string Concept { get; set; }
        [Required]
        public string Type { get; set; }

    }
}
