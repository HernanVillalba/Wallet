using System.ComponentModel.DataAnnotations;

namespace Wallet.API.Models
{
    public class TransactionEditModel
    {
        [Required]
        public string Concept { get; set; }
    }
}
