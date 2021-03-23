using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

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
        public int? USD_id { get; set; }
        [JsonIgnore]
        public int? ARS_id { get; set; }
    }
}
