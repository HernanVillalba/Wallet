//using System.Text.Json.Serialization;

using System.ComponentModel.DataAnnotations;

namespace Wallet.Entities
{
    public class UserFilterModel
    {
        //[JsonPropertyName("Nombre")]
        [MaxLength(50,ErrorMessage = "Máximo 50 carácteres")]
        public string FirstName { get; set; }
        //[JsonPropertyName("Apellido")]
        [MaxLength(50, ErrorMessage = "Máximo 50 carácteres")]
        public string LastName { get; set; }
        //[JsonPropertyName("Email")]
        [MaxLength(100,ErrorMessage ="Máximo 100 carácteres")]
        public string Email { get; set; }
    }
}
