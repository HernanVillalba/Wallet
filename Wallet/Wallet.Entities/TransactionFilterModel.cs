using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace Wallet.Entities
{
    public class TransactionFilterModel
    {
        [MaxLength(50, ErrorMessage = "Máximo 50 carácteres")]
        public string Concept { get; set; }

        [MaxLength(10, ErrorMessage = "Máximo 10 carácteres")]
        [RegularExpression("Topup|topup|Payment|payment", ErrorMessage = "Topup o Payment")]
        public string Type { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "El campo {0} debe ser mayor a cero")]
        public int? AccountId { get; set; }
    }
}
