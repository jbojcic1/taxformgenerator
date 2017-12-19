using System;
using CommandLineParser.Core;
using TaxFormGenerator.FormGenerator;

namespace TaxFormGenerator.Utilities
{
    public static class CommandLineParser
    {
        public static FluentCommandLineParser<TaxFormGeneratorArguments> GetParser()
        {
            var parser = new FluentCommandLineParser<TaxFormGeneratorArguments>();

            parser.Setup(arg => arg.FormType)
                  .As('f', "formType")
                  .SetDefault(FormType.SalaryJOPPD);

            parser.Setup(arg => arg.Date)
                  .As('d', "date")
                  .SetDefault(DateTime.Now);
            
            parser.Setup(arg => arg.Amount)
                  .As('a', "amount")
                  .SetDefault(830D);

            parser.Setup(arg => arg.Currency)
                  .As('c', "currency")
                  .SetDefault("EUR");

            parser.Setup(arg => arg.SalaryMonth)
                  .As('m', "salaryMonth")
                  .SetDefault(DateTime.Now.AddMonths(-1));

            return parser;
        }
    }
}
