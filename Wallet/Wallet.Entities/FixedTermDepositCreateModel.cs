using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace Wallet.Entities
{
    public class FixedTermDepositCreateModel
    {
        [Required(ErrorMessage = "El campo 'AccountId' es obligatorio")]
        [Range(1, int.MaxValue, ErrorMessage = "Se requiere un id de cuenta mayor a cero y menor a 2^31")]
        public int AccountId { get; set; }

        [Required(ErrorMessage = "El campo 'Amount' es obligatorio")]
        [Range(double.Epsilon, double.MaxValue, ErrorMessage = "Se requiere un monto mayor a cero y menor a 17e307")]
        public float Amount { get; set; }
    }
}
