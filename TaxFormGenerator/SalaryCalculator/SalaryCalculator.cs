using System;
using TaxFormGenerator.Utilities;

namespace TaxFormGenerator.SalaryCalculator
{
    public class SalaryCalculator : ISalaryCalculator
    {
        private readonly SalaryTax salaryTax;

        public SalaryCalculator()
        {
            this.salaryTax = ConfigReader.ReadFromFile<SalaryTax>(@"./SalaryCalculator/SalaryConfig.json");
        }

        public SalaryBreakdown Calculate(decimal grossTotal)
        {
            var salaryBreakdown = new SalaryBreakdown();

            // Ukupni trosak (bruto 2)
            salaryBreakdown.GrossTotal = grossTotal;

            // Osobni odbitak
            salaryBreakdown.NontaxableAmount = this.salaryTax.Deduction;

            // Bruto
            var gross = salaryBreakdown.GrossTotal / (
                1 + this.salaryTax.HealthInsuranceContribution
                + this.salaryTax.WorkSafetyContribution
                + this.salaryTax.EmploymentContribution);
            salaryBreakdown.Gross = Math.Round(gross, 2);

            // Doprinosi na
            salaryBreakdown.HealthInsuranceContribution = Math.Round(this.salaryTax.HealthInsuranceContribution * salaryBreakdown.Gross, 2);
            salaryBreakdown.WorkSafetyContribution = Math.Round(this.salaryTax.WorkSafetyContribution * salaryBreakdown.Gross, 2);
            salaryBreakdown.EmploymentContribution = Math.Round(this.salaryTax.EmploymentContribution * salaryBreakdown.Gross, 2);

            // Doprinosi iz
            salaryBreakdown.PensionPillar1Contribution = Math.Round(this.salaryTax.PensionPillar1Contribution * salaryBreakdown.Gross, 2);
            salaryBreakdown.PensionPillar2Contribution = Math.Round(this.salaryTax.PensionPillar2Contribution * salaryBreakdown.Gross, 2);

            // Dohodak
            salaryBreakdown.Income = salaryBreakdown.Gross - salaryBreakdown.ContributionsFrom;

            // Porez
            salaryBreakdown.Tax = Math.Round(this.salaryTax.Tax * salaryBreakdown.TaxableAmount, 2);

            // Prirez
            salaryBreakdown.Surtax = Math.Round(this.salaryTax.Surtax * salaryBreakdown.Tax, 2);

            return salaryBreakdown;
        }
    }
}
