using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;
using iTextSharp.text;
using iTextSharp.text.pdf;
using TaxFormGenerator.CurrencyConverter;
using TaxFormGenerator.DividendCalculator;
using TaxFormGenerator.Payment2DBarCodeGenerator;
using TaxFormGenerator.Utilities;

namespace TaxFormGenerator.FormGenerator.DividendJOPPD
{
    public class DividendJOPPDGenerator : JOPPDFormGenerator
    {
        protected override string TemplatePath { get => @"./FormGenerator/DividendJOPPD"; }

        private const string PaymentFilePath = @"./FormGenerator/DividendJOPPD/PaymentConfig.json";

        private readonly ICurrencyConverter currencyConverter;
        private readonly IDividendCalculator dividendCalculator;
        private readonly IPayment2DBarCodeGenerator payment2DBarCodeGenerator;

        public DividendJOPPDGenerator(
            ICurrencyConverter currencyConverter, 
            IDividendCalculator dividendCalculator, 
            IPayment2DBarCodeGenerator payment2DBarCodeGenerator)
        {
            this.currencyConverter = currencyConverter;
            this.dividendCalculator = dividendCalculator;
            this.payment2DBarCodeGenerator = payment2DBarCodeGenerator;
        }

        public override async Task Run(TaxFormGeneratorArguments arguments)
        {
            var dividendGrossAmount = await this.currencyConverter.ConvertCurrency((decimal)arguments.Amount, arguments.Currency, arguments.Date);
            var dividendBreakdown = this.dividendCalculator.Calculate(dividendGrossAmount);

            var generateJOPPDTask = GenerateJOPPD(arguments.Date, dividendBreakdown, arguments.StartDate, arguments.EndDate);
            var generatePayment2DBarcodeTask = GeneratePayment(arguments.Date, dividendBreakdown);

            await Task.WhenAll(generateJOPPDTask, generatePayment2DBarcodeTask);
        }

        private async Task GenerateJOPPD(DateTime date, DividendBreakdown dividendBreakdown, DateTime formStart, DateTime formEnd)
        {
            var JOPPDNumber = JOPPDHelper.GetJOPPDNumber(date);
            var fileName = $"dividend-{JOPPDNumber}-{date.ToString("yyyy-MM-dd")}.xml";
            var fileFullPath = Path.Combine(OutputPath, fileName);

            CopyTemplate("DividendJOPPDTemplate.xml", fileName);

            var cts = new CancellationTokenSource();
            var newJOPPD = await XElement.LoadAsync(new FileStream(fileFullPath, FileMode.Open), LoadOptions.None, cts.Token);

            newJOPPD.Element(MetadataNamespace + "Metapodaci")
                    .Element(MetadataNamespace + "Datum")
                    .SetValue(date.ToString("yyyy-MM-ddTHH:mm:ss"));

            var pageA = newJOPPD.Element(JOPPDNamespace + "StranaA");

            pageA.SetElementValue(JOPPDNamespace + "DatumIzvjesca", date.ToString("yyyy-MM-dd"));
            pageA.SetElementValue(JOPPDNamespace + "OznakaIzvjesca", JOPPDNumber);
            pageA.Element(JOPPDNamespace + "PredujamPoreza").SetElementValue(JOPPDNamespace + "P2", dividendBreakdown.TaxTotal);

            var pageB = newJOPPD.Element(JOPPDNamespace + "StranaB")
                                .Element(JOPPDNamespace + "Primatelji")
                                .Element(JOPPDNamespace + "P");

            pageB.SetElementValue(JOPPDNamespace + "P101", formStart.ToString("yyyy-MM-dd"));
            pageB.SetElementValue(JOPPDNamespace + "P102", formEnd.ToString("yyyy-MM-dd"));
            pageB.SetElementValue(JOPPDNamespace + "P11", dividendBreakdown.Gross);
            pageB.SetElementValue(JOPPDNamespace + "P133", dividendBreakdown.Gross);
            pageB.SetElementValue(JOPPDNamespace + "P135", dividendBreakdown.Gross);
            pageB.SetElementValue(JOPPDNamespace + "P141", dividendBreakdown.Tax);
            pageB.SetElementValue(JOPPDNamespace + "P142", dividendBreakdown.Surtax);
            pageB.SetElementValue(JOPPDNamespace + "P162", dividendBreakdown.Net);

            await newJOPPD.SaveAsync(new FileStream(fileFullPath, FileMode.Create), SaveOptions.None, cts.Token);
        }

        private async Task GeneratePayment(DateTime date, DividendBreakdown dividendBreakdown) 
        {
            var JOPPDNumber = JOPPDHelper.GetJOPPDNumber(date);
            var paymentInfo = new PaymentInfo(dividendBreakdown.TaxTotal, PaymentFilePath);
            paymentInfo.Receiver.Reference = $"{paymentInfo.Receiver.Reference}{JOPPDNumber}";

            var payment2DBarcode = await this.payment2DBarCodeGenerator.GeneratePayment2DBarcode(paymentInfo);

            // TODO: see if this can be made async
            using(var fs = new FileStream($"{OutputPath}/payments.pdf", FileMode.Create, FileAccess.Write, FileShare.None))
            using(var doc = new Document())
            using(var writer = PdfWriter.GetInstance(doc, fs))   
            {
                doc.Open();
                doc.Add(new Paragraph($"{date.ToString("yyyy-MM-dd")} dividend tax and surtax payment:"));
                Image image = Image.GetInstance(payment2DBarcode);
                image.ScaleToFit(300f, 60f);
                doc.Add(image);
                doc.Close();
            }
        }
    }
}
