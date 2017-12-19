using System;
using System.Threading.Tasks;
using TaxFormGenerator.Utilities;

namespace TaxFormGenerator
{
    class Program
    {
        public static async Task Main(string[] args)
        {
            var dependencyInjector = new DependencyInjector();

            var parser = Utilities.CommandLineParser.GetParser();
            var parsingResult = parser.Parse(args);

            if (!parsingResult.HasErrors) {
                var taxFormGeneratorArguments = parser.Object;
                var formGenerator = dependencyInjector.ResolveFormGenerator(taxFormGeneratorArguments.FormType);
                await formGenerator.Run(taxFormGeneratorArguments);
            }
            else {
                Console.WriteLine(parsingResult.ErrorText);
            }

        }
    }
}
