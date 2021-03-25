using System.ComponentModel.DataAnnotations;

namespace Wallet.Entities
{
    public class TransactionCreateModel
    {
        [Required]
        public float Amount { get; set; }
        [Required]
        public string Concept { get; set; }
        [Required]
        public string Type { get; set; }

    }
}
