using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;
using iTextSharp.text;
using iTextSharp.text.pdf;
using TaxFormGenerator.CurrencyConverter;
using TaxFormGenerator.Payment2DBarCodeGenerator;
using TaxFormGenerator.SalaryCalculator;
using TaxFormGenerator.Utilities;

namespace TaxFormGenerator.FormGenerator.SalaryJOPPD
{
    public class SalaryJOPPDGenerator : JOPPDFormGenerator
    {
        protected override string TemplatePath => @"./FormGenerator/SalaryJOPPD";

        private const string PensionPillar1PaymentConfigPath = @"./FormGenerator/SalaryJOPPD/PensionPillar1PaymentConfig.json";
        private const string PensionPillar2PaymentConfigPath = @"./FormGenerator/SalaryJOPPD/PensionPillar2PaymentConfig.json";
        private const string TaxAndSurtaxPaymentConfigPath = @"./FormGenerator/SalaryJOPPD/TaxAndSurtaxPaymentConfig.json";
        private const string HealthInsuranceContributionPaymentConfigPath = @"./FormGenerator/SalaryJOPPD/HealthInsuranceContributionPaymentConfig.json";
        private const string EmploymentContributionPaymentConfigPath = @"./FormGenerator/SalaryJOPPD/EmploymentContributionPaymentConfig.json";
        private const string WorkSafetyContributionPaymentConfigPath = @"./FormGenerator/SalaryJOPPD/WorkSafetyContributionPaymentConfig.json";

        private readonly ICurrencyConverter currencyConverter;
        private readonly ISalaryCalculator salaryCalculator;
        private readonly IPayment2DBarCodeGenerator payment2DBarCodeGenerator;

        public SalaryJOPPDGenerator(
            ICurrencyConverter currencyConverter, 
            ISalaryCalculator salaryCalculator,
            IPayment2DBarCodeGenerator payment2DBarCodeGenerator)
        {
            this.currencyConverter = currencyConverter;
            this.salaryCalculator = salaryCalculator;
            this.payment2DBarCodeGenerator = payment2DBarCodeGenerator;
        }

        public override async Task Run(TaxFormGeneratorArguments arguments)
        {
            var salaryGrossTotalAmount = arguments.Currency.ToLower() == "hrk"
                ? (decimal)arguments.Amount
                : await this.currencyConverter.ConvertCurrency((decimal)arguments.Amount, arguments.Currency, arguments.PaymentDate);
            var salaryBreakdown = this.salaryCalculator.Calculate(salaryGrossTotalAmount);

            var formStart = new DateTime(arguments.SalaryMonth.Value.Year, arguments.SalaryMonth.Value.Month, 1);
            var formEnd = formStart.AddMonths(1).AddDays(-1);

            var taxAndSurtaxFormTask = GenerateTaxAndSurtaxJOPPD(arguments.FormDate, salaryBreakdown, formStart, formEnd);
            var contributionsFormTask = GenerateContributionsJOPPD(arguments.FormDate, salaryBreakdown, formStart, formEnd);

            var paymentsTask = GeneratePayments(arguments.FormDate, arguments.SalaryMonth.Value, salaryBreakdown);

            await Task.WhenAll(taxAndSurtaxFormTask, contributionsFormTask, paymentsTask);
        }

        private async Task GenerateContributionsJOPPD(DateTime formDate, SalaryBreakdown salaryBreakdown, DateTime formStart, DateTime formEnd) {
            var JOPPDNumber = JOPPDHelper.GetJOPPDNumber(formDate);
            var fileName = $"doprinosi-{JOPPDNumber}-{formDate:yyyy-MM-dd}.xml";
            var fileFullPath = Path.Combine(OutputPath, fileName);

            CopyTemplate("ContributionsJOPPDTemplate.xml", fileName);

            XElement newJOPPD;

            using (var fileStream = new FileStream(fileFullPath, FileMode.Open))
            {
                var cts = new CancellationTokenSource();
                newJOPPD = await XElement.LoadAsync(fileStream, LoadOptions.None, cts.Token);  
            }

            var metadata = newJOPPD.Element(MetadataNamespace + "Metapodaci");

            metadata.Element(MetadataNamespace + "Datum").SetValue(formDate.ToString("yyyy-MM-ddTHH:mm:ss"));
            metadata.Element(MetadataNamespace + "Identifikator").SetValue(Guid.NewGuid());

            var pageA = newJOPPD.Element(JOPPDNamespace + "StranaA");

            pageA.SetElementValue(JOPPDNamespace + "DatumIzvjesca", formDate.ToString("yyyy-MM-dd"));
            pageA.SetElementValue(JOPPDNamespace + "OznakaIzvjesca", JOPPDNumber);

            var contributions = pageA.Element(JOPPDNamespace + "Doprinosi");

            contributions.Element(JOPPDNamespace + "GeneracijskaSolidarnost").SetElementValue(JOPPDNamespace + "P1", salaryBreakdown.PensionPillar1Contribution);
            contributions.Element(JOPPDNamespace + "KapitaliziranaStednja").SetElementValue(JOPPDNamespace + "P1", salaryBreakdown.PensionPillar2Contribution);

            var healthInsurance = contributions.Element(JOPPDNamespace + "ZdravstvenoOsiguranje");
            healthInsurance.SetElementValue(JOPPDNamespace + "P1", salaryBreakdown.HealthInsuranceContribution);
            healthInsurance.SetElementValue(JOPPDNamespace + "P2", salaryBreakdown.WorkSafetyContribution);

            contributions.Element(JOPPDNamespace + "Zaposljavanje").SetElementValue(JOPPDNamespace + "P1", salaryBreakdown.EmploymentContribution);

            var pageB = newJOPPD.Element(JOPPDNamespace + "StranaB")
                .Element(JOPPDNamespace + "Primatelji")
                .Element(JOPPDNamespace + "P");

            pageB.SetElementValue(JOPPDNamespace + "P101", formStart.ToString("yyyy-MM-dd"));
            pageB.SetElementValue(JOPPDNamespace + "P102", formEnd.ToString("yyyy-MM-dd"));
            pageB.SetElementValue(JOPPDNamespace + "P12", salaryBreakdown.Gross);
            pageB.SetElementValue(JOPPDNamespace + "P121", salaryBreakdown.PensionPillar1Contribution);
            pageB.SetElementValue(JOPPDNamespace + "P122", salaryBreakdown.PensionPillar2Contribution);
            pageB.SetElementValue(JOPPDNamespace + "P123", salaryBreakdown.HealthInsuranceContribution);
            pageB.SetElementValue(JOPPDNamespace + "P124", salaryBreakdown.WorkSafetyContribution);
            pageB.SetElementValue(JOPPDNamespace + "P125", salaryBreakdown.EmploymentContribution);
            pageB.SetElementValue(JOPPDNamespace + "P17", salaryBreakdown.Gross);

            using (var fileStream = new FileStream(fileFullPath, FileMode.Create))
            {
                var cts = new CancellationTokenSource();
                await newJOPPD.SaveAsync(fileStream, SaveOptions.None, cts.Token);
            }
        }

        private async Task GenerateTaxAndSurtaxJOPPD(DateTime formDate, SalaryBreakdown salaryBreakdown, DateTime formStart, DateTime formEnd) {
            var JOPPDNumber = JOPPDHelper.GetJOPPDNumber(formDate);
            var fileName = $"porezIPrirez-{JOPPDNumber}-{formDate:yyyy-MM-dd}.xml";
            var fileFullPath = Path.Combine(OutputPath, fileName);

            CopyTemplate("TaxAndSurtaxJOPPDTemplate.xml", fileName);

            XElement newJOPPD;

            using (var fileStream = new FileStream(fileFullPath, FileMode.Open))
            {
                var cts = new CancellationTokenSource();
                newJOPPD = await XElement.LoadAsync(fileStream, LoadOptions.None, cts.Token);
            }

            var metadata = newJOPPD.Element(MetadataNamespace + "Metapodaci");

            metadata.Element(MetadataNamespace + "Datum").SetValue(formDate.ToString("yyyy-MM-ddTHH:mm:ss"));
            metadata.Element(MetadataNamespace + "Identifikator").SetValue(Guid.NewGuid());

            var pageA = newJOPPD.Element(JOPPDNamespace + "StranaA");

            pageA.SetElementValue(JOPPDNamespace + "DatumIzvjesca", formDate.ToString("yyyy-MM-dd"));
            pageA.SetElementValue(JOPPDNamespace + "OznakaIzvjesca", JOPPDNumber);

            var tax = pageA.Element(JOPPDNamespace + "PredujamPoreza");
            tax.SetElementValue(JOPPDNamespace + "P1", salaryBreakdown.TaxTotal);
            tax.SetElementValue(JOPPDNamespace + "P11", salaryBreakdown.TaxTotal);

            var pageB = newJOPPD.Element(JOPPDNamespace + "StranaB")
                .Element(JOPPDNamespace + "Primatelji")
                .Element(JOPPDNamespace + "P");

            pageB.SetElementValue(JOPPDNamespace + "P101", formStart.ToString("yyyy-MM-dd"));
            pageB.SetElementValue(JOPPDNamespace + "P102", formEnd.ToString("yyyy-MM-dd"));
            pageB.SetElementValue(JOPPDNamespace + "P11", salaryBreakdown.Gross);
            pageB.SetElementValue(JOPPDNamespace + "P132", salaryBreakdown.ContributionsFrom);
            pageB.SetElementValue(JOPPDNamespace + "P133", salaryBreakdown.Income);
            pageB.SetElementValue(JOPPDNamespace + "P134", Math.Min(salaryBreakdown.NontaxableAmount, salaryBreakdown.Income));
            pageB.SetElementValue(JOPPDNamespace + "P135", salaryBreakdown.TaxableAmount);
            pageB.SetElementValue(JOPPDNamespace + "P141", salaryBreakdown.Tax);
            pageB.SetElementValue(JOPPDNamespace + "P142", salaryBreakdown.Surtax);
            pageB.SetElementValue(JOPPDNamespace + "P162", salaryBreakdown.Net);

            using (var fileStream = new FileStream(fileFullPath, FileMode.Create))
            {
                var cts = new CancellationTokenSource();
                await newJOPPD.SaveAsync(fileStream, SaveOptions.None, cts.Token);
            }
        }

        private async Task GeneratePayments(DateTime formDate, DateTime salaryMonth, SalaryBreakdown salaryBreakdown)
        {
            var JOPPDNumber = JOPPDHelper.GetJOPPDNumber(formDate);

            var contributionsPillar1PaymentBarcodeTask = GenerateContributionsPillar1Barcode(JOPPDNumber, salaryMonth, salaryBreakdown);
            var contributionsPillar2PaymentBarcodeTask = GenerateContributionsPillar2Barcode(JOPPDNumber, salaryMonth, salaryBreakdown);

            Task<byte[]> taxAndSurtaxPaymentBarcodeTask = null;
            if (salaryBreakdown.TaxTotal > 0) {
                taxAndSurtaxPaymentBarcodeTask = GenerateTaxAndSurtaxBarcode(JOPPDNumber, salaryMonth, salaryBreakdown);   
            }

            Task<byte[]> healthInsuranceContributionPaymentBarcodeTask = null;
            if (salaryBreakdown.HealthInsuranceContribution > 0) {
                healthInsuranceContributionPaymentBarcodeTask = GenerateHealthInsuranceContributionBarcode(JOPPDNumber, salaryMonth, salaryBreakdown);
            }

            Task<byte[]> workSafetyContributionPaymentBarcodeTask = null;
            if (salaryBreakdown.WorkSafetyContribution > 0) {
                workSafetyContributionPaymentBarcodeTask = GenerateWorkSafetyContributionBarcode(JOPPDNumber, salaryMonth, salaryBreakdown);
            }

            Task<byte[]> employmentContributionPaymentBarcodeTask = null;
            if (salaryBreakdown.EmploymentContribution > 0) {
                employmentContributionPaymentBarcodeTask = GenerateEmploymentContributionBarcode(JOPPDNumber, salaryMonth, salaryBreakdown);
            }

            // TODO: see if this can be made async
            using (var fs = new FileStream($"{OutputPath}/payments.pdf", FileMode.Create, FileAccess.Write, FileShare.None))
            using (var doc = new Document())
            using (var writer = PdfWriter.GetInstance(doc, fs))
            {
                doc.Open();

                doc.Add(new Paragraph($"Salary for {salaryMonth:MM/yyyy} - pension pillar 1 contribution:"));
                var pillar1PaymentBarcodeImage = Image.GetInstance(await contributionsPillar1PaymentBarcodeTask);
                pillar1PaymentBarcodeImage.ScaleToFit(300f, 60f);
                doc.Add(pillar1PaymentBarcodeImage);

                doc.Add(new Paragraph("\n\n"));

                doc.Add(new Paragraph($"Salary for {salaryMonth:MM/yyyy} - pension pillar 2 contribution:"));
                var pillar2PaymentBarcodeImage = Image.GetInstance(await contributionsPillar2PaymentBarcodeTask);
                pillar2PaymentBarcodeImage.ScaleToFit(300f, 60f);
                doc.Add(pillar2PaymentBarcodeImage);

                doc.Add(new Paragraph("\n\n"));

                if (taxAndSurtaxPaymentBarcodeTask != null) 
                {
                    doc.Add(new Paragraph($"Salary for {salaryMonth:MM/yyyy} - tax and surtax:"));
                    var taxAndSurtaxPaymentBarcodeImage = Image.GetInstance(await taxAndSurtaxPaymentBarcodeTask);
                    taxAndSurtaxPaymentBarcodeImage.ScaleToFit(300f, 60f);
                    doc.Add(taxAndSurtaxPaymentBarcodeImage);

                    doc.Add(new Paragraph("\n\n"));
                }

                if (healthInsuranceContributionPaymentBarcodeTask != null)
                {
                    doc.Add(new Paragraph($"Salary for {salaryMonth:MM/yyyy} - health insurance:"));
                    var healthInsuranceContributionPaymentBarcodeImage = Image.GetInstance(await healthInsuranceContributionPaymentBarcodeTask);
                    healthInsuranceContributionPaymentBarcodeImage.ScaleToFit(300f, 60f);
                    doc.Add(healthInsuranceContributionPaymentBarcodeImage);

                    doc.Add(new Paragraph("\n\n"));
                }

                if (workSafetyContributionPaymentBarcodeTask != null)
                {
                    doc.Add(new Paragraph($"Salary for {salaryMonth:MM/yyyy} - work safety:"));
                    var workSafetyContributionPaymentBarcodeImage = Image.GetInstance(await workSafetyContributionPaymentBarcodeTask);
                    workSafetyContributionPaymentBarcodeImage.ScaleToFit(300f, 60f);
                    doc.Add(workSafetyContributionPaymentBarcodeImage);

                    doc.Add(new Paragraph("\n\n"));
                }

                if (employmentContributionPaymentBarcodeTask != null)
                {
                    doc.Add(new Paragraph($"Salary for {salaryMonth:MM/yyyy} - employment contribution:"));
                    var employmentContributionPaymentBarcodeImage = Image.GetInstance(await employmentContributionPaymentBarcodeTask);
                    employmentContributionPaymentBarcodeImage.ScaleToFit(300f, 60f);
                    doc.Add(employmentContributionPaymentBarcodeImage);

                    doc.Add(new Paragraph("\n\n"));
                }

                doc.Close();
            }
        }

        private Task<byte[]> GenerateContributionsPillar1Barcode(string JOPPDNumber, DateTime salaryMonth, SalaryBreakdown salaryBreakdown) 
        {
            var contributionsPillar1PaymentInfo = new PaymentInfo(salaryBreakdown.PensionPillar1Contribution, PensionPillar1PaymentConfigPath);
            contributionsPillar1PaymentInfo.Receiver.Reference = $"{contributionsPillar1PaymentInfo.Receiver.Reference}{JOPPDNumber}";
            contributionsPillar1PaymentInfo.Description = $"{contributionsPillar1PaymentInfo.Description}{salaryMonth:MM/yyyy}";

            return this.payment2DBarCodeGenerator.GeneratePayment2DBarcode(contributionsPillar1PaymentInfo);
        }

        private Task<byte[]> GenerateContributionsPillar2Barcode(string JOPPDNumber, DateTime salaryMonth, SalaryBreakdown salaryBreakdown)
        {
            var contributionsPillar2PaymentInfo = new PaymentInfo(salaryBreakdown.PensionPillar2Contribution, PensionPillar2PaymentConfigPath);
            contributionsPillar2PaymentInfo.Receiver.Reference = $"{contributionsPillar2PaymentInfo.Receiver.Reference}{JOPPDNumber}";
            contributionsPillar2PaymentInfo.Description = $"{contributionsPillar2PaymentInfo.Description}{salaryMonth:MM/yyyy}";

            return this.payment2DBarCodeGenerator.GeneratePayment2DBarcode(contributionsPillar2PaymentInfo);
        }

        private Task<byte[]> GenerateTaxAndSurtaxBarcode(string JOPPDNumber, DateTime salaryMonth, SalaryBreakdown salaryBreakdown)
        {
            var taxAndSurtaxPaymentInfo = new PaymentInfo(salaryBreakdown.TaxTotal, TaxAndSurtaxPaymentConfigPath);
            taxAndSurtaxPaymentInfo.Receiver.Reference = $"{taxAndSurtaxPaymentInfo.Receiver.Reference}{JOPPDNumber}";
            taxAndSurtaxPaymentInfo.Description = $"{taxAndSurtaxPaymentInfo.Description}{salaryMonth:MM/yyyy}";

            return this.payment2DBarCodeGenerator.GeneratePayment2DBarcode(taxAndSurtaxPaymentInfo);
        }

        private Task<byte[]> GenerateHealthInsuranceContributionBarcode(string JOPPDNumber, DateTime salaryMonth, SalaryBreakdown salaryBreakdown)
        {
            var healthInsuranceContributionPaymentInfo = new PaymentInfo(salaryBreakdown.HealthInsuranceContribution, HealthInsuranceContributionPaymentConfigPath);
            healthInsuranceContributionPaymentInfo.Receiver.Reference = $"{healthInsuranceContributionPaymentInfo.Receiver.Reference}{JOPPDNumber}";
            healthInsuranceContributionPaymentInfo.Description = $"{healthInsuranceContributionPaymentInfo.Description}{salaryMonth:MM/yyyy}";

            return this.payment2DBarCodeGenerator.GeneratePayment2DBarcode(healthInsuranceContributionPaymentInfo);
        }

        private Task<byte[]> GenerateWorkSafetyContributionBarcode(string JOPPDNumber, DateTime salaryMonth, SalaryBreakdown salaryBreakdown)
        {
            var workSafetyContributionPaymentInfo = new PaymentInfo(salaryBreakdown.WorkSafetyContribution, WorkSafetyContributionPaymentConfigPath);
            workSafetyContributionPaymentInfo.Receiver.Reference = $"{workSafetyContributionPaymentInfo.Receiver.Reference}{JOPPDNumber}";
            workSafetyContributionPaymentInfo.Description = $"{workSafetyContributionPaymentInfo.Description}{salaryMonth:MM/yyyy}";

            return this.payment2DBarCodeGenerator.GeneratePayment2DBarcode(workSafetyContributionPaymentInfo);
        }

        private Task<byte[]> GenerateEmploymentContributionBarcode(string JOPPDNumber, DateTime salaryMonth, SalaryBreakdown salaryBreakdown)
        {
            var employmentContributionPaymentInfo = new PaymentInfo(salaryBreakdown.EmploymentContribution, EmploymentContributionPaymentConfigPath);
            employmentContributionPaymentInfo.Receiver.Reference = $"{employmentContributionPaymentInfo.Receiver.Reference}{JOPPDNumber}";
            employmentContributionPaymentInfo.Description = $"{employmentContributionPaymentInfo.Description}{salaryMonth:MM/yyyy}";

            return this.payment2DBarCodeGenerator.GeneratePayment2DBarcode(employmentContributionPaymentInfo);
        }
    }
}
