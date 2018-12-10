using System;
using Newtonsoft.Json;

namespace TaxFormGenerator.CurrencyConverter.HNB
{
    public class HNBCurrencyConversionInfo
    {
        [JsonProperty("Broj tečajnice")]
        public int Id { get; set; }

        [JsonProperty("Datum primjene")]
        public DateTime Date { get; set; }

        [JsonProperty("Država")]
        public string State { get; set; }

        [JsonProperty("Šifra valute")]
        public string CurrencyCode { get; set; }

        [JsonProperty("Valuta")]
        public string Currency { get; set; }

        [JsonProperty("Jedinica")]
        public int Unit { get; set; }

        [JsonProperty("Kupovni za devize")]
        public decimal BuyRate { get; set; }

        [JsonProperty("Srednji za devize")]
        public decimal MiddleRate { get; set; }

        [JsonProperty("Prodajni za devize")]
        public decimal SellRate { get; set; }
    }
}
