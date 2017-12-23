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
            dividendBreakdown.Tax = Math.Round(this.dividendTax.Tax * dividendBreakdown.Gross, 2);

            // Prirez
            dividendBreakdown.Surtax = Math.Round(this.dividendTax.Surtax * dividendBreakdown.Tax, 2);

            return dividendBreakdown;
        }
    }
}
