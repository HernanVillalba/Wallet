using System.ComponentModel.DataAnnotations;

namespace Wallet.Entities
{
    public class LoginModel
    {        
        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        [EmailAddress(ErrorMessage = "Ingrese un Email válido")]
        [StringLength(100,ErrorMessage ="El campo {0} no debe superar los 100 caracteres")]
        public string Email { get; set; }

        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        public string Password { get; set; }
    }
}
