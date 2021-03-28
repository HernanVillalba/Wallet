using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace Wallet.Entities
{
    public class FixedTermDepositCreateModel
    {
        [Required(ErrorMessage = "Se requiere un id de cuenta mayor a cero y menor a 2^31")]
        [Range(1, int.MaxValue)]
        public int AccountId { get; set; }

        [Required(ErrorMessage = "Se requiere un monto mayor a cero y menor a 17e307")]
        [Range(double.Epsilon, double.MaxValue)]
        public float Amount { get; set; }
    }
}
