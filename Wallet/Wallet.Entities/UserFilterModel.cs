//using System.Text.Json.Serialization;

namespace Wallet.Entities
{
    public class UserFilterModel
    {
        //[JsonPropertyName("Nombre")]
        public string FirstName { get; set; }
        //[JsonPropertyName("Apellido")]
        public string LastName { get; set; }
        //[JsonPropertyName("Email")]
        public string Email { get; set; }
    }
}
