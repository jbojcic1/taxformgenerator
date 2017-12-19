using System;
using Newtonsoft.Json;

namespace TaxFormGenerator.CurrencyConverter.HNB
{
    public class HNBCurrencyConversionInfo
    {
        [JsonProperty("broj_tecajnice")]
        public int Id { get; set; }

        [JsonProperty("datum")]
        public DateTime Date { get; set; }

        [JsonProperty("drzava")]
        public string State { get; set; }

        [JsonProperty("sifra_valute")]
        public string CurrencyCode { get; set; }

        [JsonProperty("valuta")]
        public string Currency { get; set; }

        [JsonProperty("kupovni_tecaj")]
        public decimal BuyRate { get; set; }

        [JsonProperty("srednji_tecaj")]
        public decimal MiddleRate { get; set; }

        [JsonProperty("prodajni_tecaj")]
        public decimal SellRate { get; set; }
    }
}
