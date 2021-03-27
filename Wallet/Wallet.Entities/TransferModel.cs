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
        [Range(1, int.MaxValue, ErrorMessage ="Se debe ingresar una cuenta de origen mayor a cero")]
        public int AccountId { get; set; }

        [Range(0.01, double.MaxValue, ErrorMessage = "Se debe ingresar un monto mayor a cero")]        
        public double Amount { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Se debe ingresar una cuenta de destino mayor a cero")]
        public int RecipientAccountId { get; set; }
    }
}
