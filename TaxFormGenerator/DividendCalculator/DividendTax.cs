using Newtonsoft.Json;

namespace TaxFormGenerator.DividendCalculator
{
    public class DividendTax
    {
        [JsonProperty("porez")]
        public decimal Tax { get; set; }

        [JsonProperty("prirez")]
        public decimal Surtax { get; set; }
    }
}
