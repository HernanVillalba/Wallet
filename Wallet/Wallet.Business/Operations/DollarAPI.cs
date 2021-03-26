using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace Wallet.Business.Operations
{
    public class DollarAPI
    {
        public DollarAPI()
        {

        }
        [JsonPropertyName("compra")]
        public double Compra { get; set; }
        [JsonPropertyName("venta")]
        public double Venta { get; set; }
        [JsonPropertyName("agencia")]
        public string Agencia { get; set; }
        [JsonPropertyName("nombre")]
        public string Nombre { get; set; }
        [JsonPropertyName("variacion")]
        public double Variacion { get; set; }
        [JsonPropertyName("ventaCero")]
        public string ventaCero { get; set; }
        [JsonPropertyName("decimales")]
        public double Decimales { get; set; }
    }
}
