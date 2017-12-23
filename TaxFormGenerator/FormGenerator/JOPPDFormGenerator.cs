using System.Xml.Linq;

namespace TaxFormGenerator.FormGenerator
{
    public abstract class JOPPDFormGenerator : FormGenerator
    {
        protected readonly XNamespace JOPPDNamespace = "http://e-porezna.porezna-uprava.hr/sheme/zahtjevi/ObrazacJOPPD/v1-1";
    }
}
