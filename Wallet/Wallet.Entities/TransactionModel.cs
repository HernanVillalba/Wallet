using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Wallet.Entities
{
    public class TransactionModel
    {
        //[JsonIgnore(Condition = JsonIgnoreCondition.)]
        [Required]
        public float Amount { get; set; }
        [Required]
        public string Concept { get; set; }
        [Required]
        public string Type { get; set; }

    }
}
