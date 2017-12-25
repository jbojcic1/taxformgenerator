using System;
using System.Net.Http;
using System.Threading.Tasks;
using TaxFormGenerator.Utilities;

namespace TaxFormGenerator.Payment2DBarCodeGenerator.HUB3
{
    public class HUB3Payment2DBarCodeGenerator : IPayment2DBarCodeGenerator
    {
        private const string ApiUrl = "https://hub3.bigfish.software/api/v1/barcode";

        private readonly HttpClient httpClient;

        public HUB3Payment2DBarCodeGenerator(HttpClient httpClient)
        {
            this.httpClient = httpClient;
        }

        public async Task<byte[]> GeneratePayment2DBarcode(PaymentInfo paymentInfo)
        {
            var hub3Payment2DBarcodeInfo = new HUB3Payment2DBarcodeInfo();
            hub3Payment2DBarcodeInfo.Data = paymentInfo;

            var response = await this.httpClient.PostAsync(ApiUrl, HttpClientHelper.GetJsonHttpContent(hub3Payment2DBarcodeInfo));
            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
            }
            response.EnsureSuccessStatusCode();

            return await response.Content.ReadAsByteArrayAsync();
        }
    }
}
