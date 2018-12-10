using System;
using TaxFormGenerator.FormGenerator;

namespace TaxFormGenerator
{
    public class TaxFormGeneratorArguments
    {
        public FormType FormType { get; set; }
        public DateTime FormDate { get; set; }
        public DateTime PaymentDate { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public double Amount { get; set; } // We have to use double instead of decimal as currently FluentCommandLineParser doesn't support decimal
        public string Currency { get; set; }
        public DateTime? SalaryMonth { get; set; }
    }
}
