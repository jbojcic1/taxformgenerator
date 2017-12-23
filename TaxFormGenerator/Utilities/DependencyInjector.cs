using System;
using System.Net.Http;
using Microsoft.Extensions.DependencyInjection;
using TaxFormGenerator.CurrencyConverter;
using TaxFormGenerator.CurrencyConverter.HNB;
using TaxFormGenerator.DividendCalculator;
using TaxFormGenerator.FormGenerator;
using TaxFormGenerator.FormGenerator.DividendJOPPD;
using TaxFormGenerator.FormGenerator.SalaryJOPPD;
using TaxFormGenerator.SalaryCalculator;

namespace TaxFormGenerator.Utilities
{
    public class DependencyInjector
    {
        private ServiceProvider serviceProvider;

        public DependencyInjector() {
            //setup our DI
            this.serviceProvider = new ServiceCollection()
                .AddLogging()
                .AddSingleton(sp => new HttpClient())
                .AddTransient<ICurrencyConverter, HNBCurrencyConverter>()
                .AddTransient<ISalaryCalculator, SalaryCalculator.SalaryCalculator>()
                .AddTransient<IDividendCalculator, DividendCalculator.DividendCalculator>()
                .AddTransient<SalaryJOPPDGenerator, SalaryJOPPDGenerator>()
                .AddTransient<DividendJOPPDGenerator, DividendJOPPDGenerator>()
                .BuildServiceProvider();

            //configure console logging
            //serviceProvider
            //.GetService<ILoggerFactory>();
            //.AddConsole(LogLevel.Debug);
        }

        public IFormGenerator ResolveFormGenerator(FormType formType) {
            // TODO: this is needed because default .NET core dependency injection doesn't support resolving by key.
            // Switch to using more powerful lib later
            switch (formType) {
                case FormType.SalaryJOPPD:
                    return this.serviceProvider.GetService<SalaryJOPPDGenerator>();
                case FormType.DividendJOPPD:
                    return this.serviceProvider.GetService<DividendJOPPDGenerator>();
                default:
                    throw new ArgumentOutOfRangeException(nameof(formType), "Unsupported form type.");
            }
        }
    }
}
