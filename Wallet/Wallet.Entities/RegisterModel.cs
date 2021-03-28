using System.ComponentModel.DataAnnotations;

namespace Wallet.Entities
{
    public class RegisterModel
    {
        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        [StringLength(50, ErrorMessage = "El campo {0} no debe superar los 50 caracteres")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        [StringLength(50, ErrorMessage = "El campo {0} no debe superar los 50 caracteres")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        [EmailAddress(ErrorMessage = "Ingrese un Email válido")]
        [StringLength(100, ErrorMessage = "El campo {0} no debe superar los 100 caracteres")]
        public string Email { get; set; }

        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        public string Password { get; set; }
    }
}
