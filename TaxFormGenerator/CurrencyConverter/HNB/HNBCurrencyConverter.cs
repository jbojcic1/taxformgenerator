﻿using System;
using System.Globalization;
using System.Net.Http;
using System.Threading.Tasks;
using TaxFormGenerator.Utilities;

namespace TaxFormGenerator.CurrencyConverter.HNB
{
    public class HNBCurrencyConverter : ICurrencyConverter
    {
        private readonly CultureInfo culture;
        private readonly HttpClient httpClient;

        public HNBCurrencyConverter(HttpClient httpClient)
        {
            this.culture = new CultureInfo("hr-HR");
            this.httpClient = httpClient;
            this.httpClient.BaseAddress = new Uri("http://api.hnb.hr/");
        }

        public async Task<decimal> ConvertCurrency(decimal amount, string currency, DateTime date)
        {
            var dateString = date.ToString("yyyy-MM-dd");
            var response = await this.httpClient.GetAsync($"tecajn?valuta={currency}&datum-od={dateString}&datum-do={dateString}");
            response.EnsureSuccessStatusCode();

            var currencyConversionResponse = await response.Content.ReadAsJsonAsync<HNBCurrencyConversionInfo[]>(this.culture);
            return Math.Round(amount * currencyConversionResponse[0].MiddleRate, 2);
        }
    }
}
