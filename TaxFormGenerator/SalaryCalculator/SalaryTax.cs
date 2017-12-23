using Newtonsoft.Json;

namespace TaxFormGenerator.SalaryCalculator
{
    public class SalaryTax
    {
        [JsonProperty("osobniOdbitak")]
        public decimal Deduction { get; set; }

        [JsonProperty("prirez")]
        public decimal Surtax { get; set; }

        [JsonProperty("porez")]
        public decimal Tax { get; set; }

        [JsonProperty("doprinosZaZdravstvenoOsiguranje")]
        public decimal HealthInsuranceContribution { get; set; }

        [JsonProperty("doprinosZaZastituNaRadu")]
        public decimal WorkSafetyContribution { get; set; }

        [JsonProperty("doprinosZaZaposljavanje")]
        public decimal EmploymentContribution { get; set; }

        [JsonProperty("doprinosZaMirovinskoStup1")]
        public decimal PensionPillar1Contribution { get; set; }

        [JsonProperty("doprinosZaMirovinskoStup2")]
        public decimal PensionPillar2Contribution { get; set; }
    }
}
