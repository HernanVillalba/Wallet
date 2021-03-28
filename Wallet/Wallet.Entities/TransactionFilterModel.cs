using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Wallet.Entities
{
    public class TransactionFilterModel
    {
        [Required]
        [MaxLength(50, ErrorMessage = "Máximo 50 carácteres")]
        public string Concept { get; set; }
        [Required]
        [MaxLength(10, ErrorMessage = "Máximo 10 carácteres")]
        [RegularExpression("Topup|topup|Payment|payment", ErrorMessage = "Topup o Payment")]
        public string Type { get; set; }
        public int? AccountId { get; set; }
        [JsonIgnore]
        public int USD_Id { get; set; }
        [JsonIgnore]
        public int ARS_Id { get; set; }
    }
}
