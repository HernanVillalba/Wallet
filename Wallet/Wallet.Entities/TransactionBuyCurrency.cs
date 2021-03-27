using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace Wallet.Entities
{
    public class TransactionBuyCurrency
    {
        [Required(ErrorMessage = "Tipo requerido (compra o venta)")]
        [MaxLength(50, ErrorMessage = "Máximo 50 carácteres")]
        [RegularExpression("Compra|Venta", ErrorMessage = "Solo se acepta Compra o Venta")]
        public string Type { get; set; }

        [Required(ErrorMessage = "Tipo de moneda requerido (ARS o USD)")]
        [MaxLength(3, ErrorMessage = "Máximo 3 carácteres")]
        [RegularExpression("USD|ARS|usd|ars", ErrorMessage = "Solo se permite ARS o USD")]
        public string Currency { get; set; }

        [Required(ErrorMessage = "Monto requerido")]
        public double Amount { get; set; }

    }

}
