using Newtonsoft.Json;

namespace TaxFormGenerator.Payment2DBarCodeGenerator.HUB3
{
    public class HUB3Payment2DBarcodeInfo
    {
        [JsonProperty("renderer")]
        public string Renderer { get; set; }

        [JsonProperty("options")]
        public HUB3RendererOptions Options { get; set; }

        [JsonProperty("data")]
        public PaymentInfo Data { get; set; }

        public HUB3Payment2DBarcodeInfo() {
            Renderer = "image";
            Options = new HUB3RendererOptions();
        }
    }
}