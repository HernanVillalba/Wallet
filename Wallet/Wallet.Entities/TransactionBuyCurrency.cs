using System.ComponentModel.DataAnnotations;

namespace Wallet.Entities
{
    public class TransactionBuyCurrency
    {
        [Required(ErrorMessage ="Tipo de moneda requerido (ARS o USD)")]
        [MaxLength(3)]
        public string Currency { get; set; }

        [Required(ErrorMessage = "Monto requerido")]
        public double Amount { get; set; }

        [Required(ErrorMessage ="Campo requerido (Payment o Topup)")]
        [MaxLength(50)]
        public string Type { get; set; }
    }
}
