using System.ComponentModel.DataAnnotations;

namespace Wallet.Entities
{
    public class TransactionCreateModel
    {
        [Required]
        public double Amount { get; set; }
        [Required]
        [MaxLength(50, ErrorMessage = "Máximo 50 carácteres")]
        public string Concept { get; set; }
        [Required]
        [MaxLength(10, ErrorMessage = "Máximo 10 carácteres")]
        [RegularExpression("Topup|topup|Payment|payment", ErrorMessage = "Solo se permite Topup o Payment")]
        public string Type { get; set; }
        [Required]
        public int AccountId { get; set; }

    }
}
