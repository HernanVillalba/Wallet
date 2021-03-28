using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Wallet.Entities
{
    public class TransferModel
    {
        //[RegularExpression("^[0-9]*$", ErrorMessage = "Se debe ingresar un número entero")]
        [Range(1, int.MaxValue, ErrorMessage ="El campo {0} debe ser mayor a cero")]
        public int AccountId { get; set; }

        [Range(0.01, double.MaxValue, ErrorMessage = "El campo {0} debe ser mayor a cero")]        
        public double Amount { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "El campo {0} debe ser mayor a cero")]
        public int RecipientAccountId { get; set; }
    }
}
