using Newtonsoft.Json;

namespace TaxFormGenerator.Payment2DBarCodeGenerator.HUB3
{
    public class HUB3Payment2DBarCodeGeneratorError
    {
        [JsonProperty("message")]
        public string Message { get; set; }

        [JsonProperty("errors")]
        public string[] Errors { get; set; }
    }
}