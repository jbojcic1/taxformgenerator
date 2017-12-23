namespace TaxFormGenerator.DividendCalculator
{
    public interface IDividendCalculator
    {
        DividendBreakdown Calculate(decimal grossAmount);
    }
}
