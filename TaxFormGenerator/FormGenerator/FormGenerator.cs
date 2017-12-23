using System.IO;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace TaxFormGenerator.FormGenerator
{
    public abstract class FormGenerator : IFormGenerator
    {
        protected readonly XNamespace MetadataNamespace = "http://e-porezna.porezna-uprava.hr/sheme/Metapodaci/v2-0";

        protected abstract string TemplatePath { get; }
        protected virtual string OutputPath { get => @"./Output"; }

        protected virtual void CopyTemplate(string sourceFileName, string destinationFileName)
        {
            var sourceFile = Path.Combine(TemplatePath, sourceFileName);
            var destinationFile = Path.Combine(OutputPath, destinationFileName);
            File.Copy(sourceFile, destinationFile, true);
        }

        public abstract Task Run(TaxFormGeneratorArguments arguments);
    }
}
