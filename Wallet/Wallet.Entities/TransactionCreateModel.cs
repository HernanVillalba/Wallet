using System.ComponentModel.DataAnnotations;

namespace Wallet.Entities
{
    public class TransactionCreateModel
    {
        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "El campo {0} debe ser mayor a cero")]
        public double Amount { get; set; }
        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        [MaxLength(50, ErrorMessage = "Máximo 50 carácteres")]
        public string Concept { get; set; }
        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        [MaxLength(10, ErrorMessage = "Máximo 10 carácteres")]
        [RegularExpression("Topup|topup|Payment|payment", ErrorMessage = "Solo se permite Topup o Payment")]
        public string Type { get; set; }
        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "El campo {0} debe ser mayor a cero")]
        public int AccountId { get; set; }

    }
}
