namespace TaxFormGenerator.SalaryCalculator
{
    public class SalaryBreakdown
    {
        public decimal GrossTotal { get; set; }

        public decimal HealthInsuranceContribution { get; set; }
        public decimal WorkSafetyContribution { get; set; }
        public decimal EmploymentContribution { get; set; }

        public decimal ContributionsOn => HealthInsuranceContribution + WorkSafetyContribution + EmploymentContribution;

        public decimal PensionPillar1Contribution { get; set; }
        public decimal PensionPillar2Contribution { get; set; }

        public decimal ContributionsFrom => PensionPillar1Contribution + PensionPillar2Contribution;

        public decimal Gross { get; set; }
        public decimal Income { get; set; }
        public decimal NontaxableAmount { get; set; }
        public decimal TaxableAmount => (Income - NontaxableAmount) < 0 ? 0 : (Income - NontaxableAmount);

        public decimal Tax { get; set; }
        public decimal Surtax { get; set; }
        public decimal TaxTotal => Tax + Surtax;

        public decimal Net => Income - TaxTotal;
    }
}
