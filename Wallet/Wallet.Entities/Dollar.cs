using System.Text.Json.Serialization;

namespace Wallet.Entities
{

    public class Dollar
    {
        [JsonPropertyName("casa")]
        public Casa Casa;
    }

    public class Casa
    {
        [JsonPropertyName("compra")]
        public string Compra;

        [JsonPropertyName("venta")]
        public string Venta;

        [JsonPropertyName("agencia")]
        public string Agencia;

        [JsonPropertyName("nombre")]
        public string Nombre;

        [JsonPropertyName("variacion")]
        public string Variacion;

        [JsonPropertyName("ventaCero")]
        public string VentaCero;

        [JsonPropertyName("decimales")]
        public string Decimales;
    }
}
