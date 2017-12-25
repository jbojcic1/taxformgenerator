using Newtonsoft.Json;

namespace TaxFormGenerator.Payment2DBarCodeGenerator.HUB3
{
    public class HUB3RendererOptions
    {
        [JsonProperty("format")]
        public string Format { get; set; }

        [JsonProperty("color")]
        public string Color { get; set; }

        public HUB3RendererOptions() {
            Format = "png";
            Color = "#000000";
        }
    }
}
