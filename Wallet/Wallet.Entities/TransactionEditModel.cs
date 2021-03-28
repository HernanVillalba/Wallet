using System.ComponentModel.DataAnnotations;

namespace Wallet.Entities
{
    public class TransactionEditModel
    {
        [Required]
        [MaxLength(50, ErrorMessage = "Máximo 50 carácteres")]
        public string Concept { get; set; }
    }
}
