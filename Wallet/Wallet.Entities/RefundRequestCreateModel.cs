using System.ComponentModel.DataAnnotations;

namespace Wallet.Entities
{
    public class RefundRequestCreateModel
    {
        [Required]
        [Range(0.01, int.MaxValue, ErrorMessage = "El campo {0} debe ser mayor a cero")]
        public int TransactionId { get; set; }
        //[Required]
        //[Range(0.01, int.MaxValue, ErrorMessage = "El campo {0} debe ser mayor a cero")]
        //public int UserId { get; set; }
    }
}
