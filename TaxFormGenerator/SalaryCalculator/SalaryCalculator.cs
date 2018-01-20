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
            salaryBreakdown.Gross = gross.Round();

            // Doprinosi na
            salaryBreakdown.HealthInsuranceContribution = (this.salaryTax.HealthInsuranceContribution * salaryBreakdown.Gross).Round();
            salaryBreakdown.WorkSafetyContribution = (this.salaryTax.WorkSafetyContribution * salaryBreakdown.Gross).Round();
            salaryBreakdown.EmploymentContribution = (this.salaryTax.EmploymentContribution * salaryBreakdown.Gross).Round();

            // Doprinosi iz
            salaryBreakdown.PensionPillar1Contribution = (this.salaryTax.PensionPillar1Contribution * salaryBreakdown.Gross).Round();
            salaryBreakdown.PensionPillar2Contribution = (this.salaryTax.PensionPillar2Contribution * salaryBreakdown.Gross).Round();

            // Dohodak
            salaryBreakdown.Income = salaryBreakdown.Gross - salaryBreakdown.ContributionsFrom;

            // Porez
            salaryBreakdown.Tax = (this.salaryTax.Tax * salaryBreakdown.TaxableAmount).Round();

            // Prirez
            salaryBreakdown.Surtax = (this.salaryTax.Surtax * salaryBreakdown.Tax).Round();

            return salaryBreakdown;
        }
    }
}
