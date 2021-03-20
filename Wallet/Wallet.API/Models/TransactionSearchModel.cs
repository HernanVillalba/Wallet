using System.ComponentModel.DataAnnotations;

namespace Wallet.API.Models
{
    public class TransactionSearchModel
    {
        [Required]
        public string Concept { get; set; }
        [Required]
        public string Type { get; set; }
        public int? AccountId { get; set; }
    }
}
