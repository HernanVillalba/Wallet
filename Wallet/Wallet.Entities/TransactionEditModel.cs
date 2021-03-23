using System.ComponentModel.DataAnnotations;

namespace Wallet.Entities
{
    public class TransactionEditModel
    {
        [Required]
        public string Concept { get; set; }
    }
}
