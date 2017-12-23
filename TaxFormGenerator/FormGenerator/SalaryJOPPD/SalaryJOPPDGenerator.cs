using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;
using TaxFormGenerator.CurrencyConverter;
using TaxFormGenerator.SalaryCalculator;
using TaxFormGenerator.Utilities;

namespace TaxFormGenerator.FormGenerator.SalaryJOPPD
{
    public class SalaryJOPPDGenerator : JOPPDFormGenerator
    {
        protected override string TemplatePath { get => @"./FormGenerator/SalaryJOPPD"; }

        private readonly ICurrencyConverter currencyConverter;
        private readonly ISalaryCalculator salaryCalculator;

        public SalaryJOPPDGenerator(ICurrencyConverter currencyConverter, ISalaryCalculator salaryCalculator)
        {
            this.currencyConverter = currencyConverter;
            this.salaryCalculator = salaryCalculator;
        }

        public override async Task Run(TaxFormGeneratorArguments arguments)
        {
            var salaryGrossTotalAmount = await this.currencyConverter.ConvertCurrency((decimal)arguments.Amount, arguments.Currency, arguments.Date);
            var salaryBreakdown = this.salaryCalculator.Calculate(salaryGrossTotalAmount);

            var formStart = new DateTime(arguments.SalaryMonth.Value.Year, arguments.SalaryMonth.Value.Month, 1);
            var formEnd = formStart.AddMonths(1).AddDays(-1);

            var taxAndSurtaxTask = GenerateTaxAndSurtaxJOPPD(arguments.Date, salaryBreakdown, formStart, formEnd);
            var contributionsTask = GenerateContributionsJOPPD(arguments.Date, salaryBreakdown, formStart, formEnd);

            await Task.WhenAll(taxAndSurtaxTask, contributionsTask);
        }

        private async Task GenerateContributionsJOPPD(DateTime date, SalaryBreakdown salaryBreakdown, DateTime formStart, DateTime formEnd) {
            var JOPPDNumber = JOPPDHelper.GetJOPPDNumber(date);
            var fileName = $"doprinosi-{JOPPDNumber}-{date.ToString("yyyy-MM-dd")}.xml";
            var fileFullPath = Path.Combine(OutputPath, fileName);

            CopyTemplate("ContributionsJOPPDTemplate.xml", fileName);

            var cts = new CancellationTokenSource(); 
            var newJOPPD = await XElement.LoadAsync(new FileStream(fileFullPath, FileMode.Open), LoadOptions.None, cts.Token);

            newJOPPD.Element(MetadataNamespace + "Metapodaci")
                    .Element(MetadataNamespace + "Datum")
                    .SetValue(date.ToString("yyyy-MM-ddTHH:mm:ss"));

            var pageA = newJOPPD.Element(JOPPDNamespace + "StranaA");

            pageA.SetElementValue(JOPPDNamespace + "DatumIzvjesca", date.ToString("yyyy-MM-dd"));
            pageA.SetElementValue(JOPPDNamespace + "OznakaIzvjesca", JOPPDNumber);

            var doprinosi = pageA.Element(JOPPDNamespace + "Doprinosi");

            doprinosi.Element(JOPPDNamespace + "GeneracijskaSolidarnost").SetElementValue(JOPPDNamespace + "P1", salaryBreakdown.PensionPillar1Contribution);
            doprinosi.Element(JOPPDNamespace + "KapitaliziranaStednja").SetElementValue(JOPPDNamespace + "P1", salaryBreakdown.PensionPillar2Contribution);

            var pageB = newJOPPD.Element(JOPPDNamespace + "StranaB")
                                .Element(JOPPDNamespace + "Primatelji")
                                .Element(JOPPDNamespace + "P");

            pageB.SetElementValue(JOPPDNamespace + "P101", formStart.ToString("yyyy-MM-dd"));
            pageB.SetElementValue(JOPPDNamespace + "P102", formEnd.ToString("yyyy-MM-dd"));
            pageB.SetElementValue(JOPPDNamespace + "P12", salaryBreakdown.GrossTotal);
            pageB.SetElementValue(JOPPDNamespace + "P121", salaryBreakdown.PensionPillar1Contribution);
            pageB.SetElementValue(JOPPDNamespace + "P122", salaryBreakdown.PensionPillar2Contribution);
            pageB.SetElementValue(JOPPDNamespace + "P17", salaryBreakdown.GrossTotal);

            await newJOPPD.SaveAsync(new FileStream(fileFullPath, FileMode.Create), SaveOptions.None, cts.Token);
        }

        private async Task GenerateTaxAndSurtaxJOPPD(DateTime date, SalaryBreakdown salaryBreakdown, DateTime formStart, DateTime formEnd) {
            var JOPPDNumber = JOPPDHelper.GetJOPPDNumber(date);
            var fileName = $"porezIPrirez-{JOPPDNumber}-{date.ToString("yyyy-MM-dd")}.xml";
            var fileFullPath = Path.Combine(OutputPath, fileName);

            CopyTemplate("TaxAndSurtaxJOPPDTemplate.xml", fileName);

            var cts = new CancellationTokenSource();
            var newJOPPD = await XElement.LoadAsync(new FileStream(fileFullPath, FileMode.Open), LoadOptions.None, cts.Token);

            newJOPPD.Element(MetadataNamespace + "Metapodaci")
                    .Element(MetadataNamespace + "Datum")
                    .SetValue(date.ToString("yyyy-MM-ddTHH:mm:ss"));

            var pageA = newJOPPD.Element(JOPPDNamespace + "StranaA");

            pageA.SetElementValue(JOPPDNamespace + "DatumIzvjesca", date.ToString("yyyy-MM-dd"));
            pageA.SetElementValue(JOPPDNamespace + "OznakaIzvjesca", JOPPDNumber);

            var tax = pageA.Element(JOPPDNamespace + "PredujamPoreza");
            tax.SetElementValue(JOPPDNamespace + "P1", salaryBreakdown.TaxTotal);
            tax.SetElementValue(JOPPDNamespace + "P11", salaryBreakdown.TaxTotal);

            var pageB = newJOPPD.Element(JOPPDNamespace + "StranaB")
                                .Element(JOPPDNamespace + "Primatelji")
                                .Element(JOPPDNamespace + "P");

            pageB.SetElementValue(JOPPDNamespace + "P101", formStart.ToString("yyyy-MM-dd"));
            pageB.SetElementValue(JOPPDNamespace + "P102", formEnd.ToString("yyyy-MM-dd"));
            pageB.SetElementValue(JOPPDNamespace + "P11", salaryBreakdown.GrossTotal);
            pageB.SetElementValue(JOPPDNamespace + "P132", salaryBreakdown.ContributionsFrom);
            pageB.SetElementValue(JOPPDNamespace + "P133", salaryBreakdown.Income);
            pageB.SetElementValue(JOPPDNamespace + "P134", salaryBreakdown.NontaxableAmount);
            pageB.SetElementValue(JOPPDNamespace + "P135", salaryBreakdown.TaxableAmount);
            pageB.SetElementValue(JOPPDNamespace + "P141", salaryBreakdown.Tax);
            pageB.SetElementValue(JOPPDNamespace + "P142", salaryBreakdown.Surtax);
            pageB.SetElementValue(JOPPDNamespace + "P162", salaryBreakdown.Net);

            await newJOPPD.SaveAsync(new FileStream(fileFullPath, FileMode.Create), SaveOptions.None, cts.Token);
        }
    }
}
