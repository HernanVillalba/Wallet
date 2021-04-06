using System.ComponentModel.DataAnnotations;

namespace Wallet.Entities
{
    public class TransactionBuyCurrency
    {
        [Required(ErrorMessage = "Tipo requerido (compra o venta)")]
        [MaxLength(10, ErrorMessage = "Máximo 10 carácteres")]
        [RegularExpression("Compra|Venta", ErrorMessage = "Solo se acepta Compra o Venta")]
        public string Type { get; set; }

        [Required(ErrorMessage = "Monto requerido")]
        [Range(0.01, double.MaxValue, ErrorMessage = "El campo {0} debe ser mayor a cero")]
        public double Amount { get; set; }

    }

}
