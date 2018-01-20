using System;
using TaxFormGenerator.Utilities;

namespace TaxFormGenerator.DividendCalculator
{
    public class DividendCalculator : IDividendCalculator
    {
        private readonly DividendTax dividendTax;

        public DividendCalculator()
        {
            this.dividendTax = ConfigReader.ReadFromFile<DividendTax>(@"./DividendCalculator/DividendConfig.json");
        }

        public DividendBreakdown Calculate(decimal grossAmount)
        {
            var dividendBreakdown = new DividendBreakdown();

            // Ukupni iznos 
            dividendBreakdown.Gross = grossAmount;

            // Porez
            dividendBreakdown.Tax = (this.dividendTax.Tax * dividendBreakdown.Gross).Round();

            // Prirez
            dividendBreakdown.Surtax = (this.dividendTax.Surtax * dividendBreakdown.Tax).Round();

            return dividendBreakdown;
        }
    }
}
