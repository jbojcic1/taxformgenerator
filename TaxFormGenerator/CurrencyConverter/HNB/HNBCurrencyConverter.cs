using System;
using System.Globalization;
using System.Net.Http;
using System.Threading.Tasks;
using TaxFormGenerator.Utilities;

namespace TaxFormGenerator.CurrencyConverter.HNB
{
    public class HNBCurrencyConverter : ICurrencyConverter
    {
        private const string ApiUrl = "http://api.hnb.hr/tecajn";

        private readonly CultureInfo culture;
        private readonly HttpClient httpClient;

        public HNBCurrencyConverter(HttpClient httpClient)
        {
            this.culture = new CultureInfo("hr-HR");
            this.httpClient = httpClient;
        }

        public async Task<decimal> ConvertCurrency(decimal amount, string currency, DateTime date)
        {
            var dateString = date.ToString("yyyy-MM-dd");
            var response = await this.httpClient.GetAsync($"{ApiUrl}?valuta={currency}&datum-od={dateString}&datum-do={dateString}");
            response.EnsureSuccessStatusCode();

            var currencyConversionResponse = await response.Content.ReadAsJsonAsync<HNBCurrencyConversionInfo[]>(this.culture);
            return (amount * currencyConversionResponse[0].MiddleRate).Round();
        }
    }
}
