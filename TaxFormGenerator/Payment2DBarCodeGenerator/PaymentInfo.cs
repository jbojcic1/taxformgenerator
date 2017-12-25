using Newtonsoft.Json;
using TaxFormGenerator.Utilities;

namespace TaxFormGenerator.Payment2DBarCodeGenerator
{
    public class PaymentInfo
    {
        [JsonProperty("amount")]
        public decimal Amount { get; set; }

        [JsonProperty("sender")]
        public PaymentSubject Sender { get; set; }

        [JsonProperty("receiver")]
        public PaymentReceiver Receiver { get; set; }

        [JsonProperty("purpose")]
        public string Purpose { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        public PaymentInfo() {}

        public PaymentInfo(decimal amount, string paymentConfigFilePath) {
            Amount = amount;
            var paymentInfo = ConfigReader.ReadFromFile<PaymentInfo>(paymentConfigFilePath);
            Sender = paymentInfo.Sender;
            Receiver = paymentInfo.Receiver;
            Purpose = paymentInfo.Purpose;
            Description = paymentInfo.Description;
        }
    }
}
