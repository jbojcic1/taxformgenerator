namespace TaxFormGenerator.SalaryCalculator
{
    public interface ISalaryCalculator
    {
        SalaryBreakdown Calculate(decimal grossTotal);
    }
}
