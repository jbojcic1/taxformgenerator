using System;
using System.Threading.Tasks;

namespace TaxFormGenerator.Payment2DBarCodeGenerator
{
    public interface IPayment2DBarCodeGenerator
    {
        Task<Byte[]> GeneratePayment2DBarcode(PaymentInfo paymentInfo);
    }
}
