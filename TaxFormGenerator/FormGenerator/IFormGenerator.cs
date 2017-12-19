using System;
using System.Threading.Tasks;

namespace TaxFormGenerator.FormGenerator
{
    public interface IFormGenerator
    {
        Task Run(TaxFormGeneratorArguments arguments);
    }
}
