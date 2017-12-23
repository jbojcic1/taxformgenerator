using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;
using TaxFormGenerator.CurrencyConverter;
using TaxFormGenerator.DividendCalculator;
using TaxFormGenerator.Utilities;

namespace TaxFormGenerator.FormGenerator.DividendJOPPD
{
    public class DividendJOPPDGenerator : JOPPDFormGenerator
    {
        protected override string TemplatePath { get => @"./FormGenerator/DividendJOPPD"; }

        private readonly ICurrencyConverter currencyConverter;
        private readonly IDividendCalculator dividendCalculator;

        public DividendJOPPDGenerator(ICurrencyConverter currencyConverter, IDividendCalculator dividendCalculator)
        {
            this.currencyConverter = currencyConverter;
            this.dividendCalculator = dividendCalculator;
        }

        public override async Task Run(TaxFormGeneratorArguments arguments)
        {
            var dividendGrossAmount = await this.currencyConverter.ConvertCurrency((decimal)arguments.Amount, arguments.Currency, arguments.Date);
            var dividendBreakdown = this.dividendCalculator.Calculate(dividendGrossAmount);

            await GenerateJOPPD(arguments.Date, dividendBreakdown, arguments.StartDate, arguments.EndDate);
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
    }
}
