using Newtonsoft.Json;

namespace TaxFormGenerator.Payment2DBarCodeGenerator
{
    public class PaymentSubject
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("street")]
        public string Street { get; set; }

        [JsonProperty("place")]
        public string Place { get; set; }
    }
}
