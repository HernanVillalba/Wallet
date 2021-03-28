using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Wallet.Entities
{
    public class TransactionFilterModel
    {
        [Required]
        public string Concept { get; set; }
        [Required]
        public string Type { get; set; }
        public int? AccountId { get; set; }
        [JsonIgnore]
        public int USD_Id { get; set; }
        [JsonIgnore]
        public int ARS_Id { get; set; }
    }
}
