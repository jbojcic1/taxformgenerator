using Newtonsoft.Json;

namespace TaxFormGenerator.Payment2DBarCodeGenerator
{
    public class PaymentReceiver : PaymentSubject
    {
        [JsonProperty("iban")]
        public string IBAN { get; set; }

        [JsonProperty("model")]
        public string Model { get; set; }

        [JsonProperty("reference")]
        public string Reference { get; set; }
    }
}
