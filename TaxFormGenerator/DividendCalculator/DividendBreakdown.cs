namespace TaxFormGenerator.DividendCalculator
{
    public class DividendBreakdown
    {
        public decimal Gross { get; set; }

        public decimal Tax { get; set; }
        public decimal Surtax { get; set; }
        public decimal TaxTotal => Tax + Surtax;

        public decimal Net => Gross - TaxTotal;
    }
}
