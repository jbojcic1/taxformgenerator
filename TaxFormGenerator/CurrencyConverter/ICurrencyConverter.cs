using System;
using System.Threading.Tasks;

namespace TaxFormGenerator.CurrencyConverter
{
    public interface ICurrencyConverter
    {
        Task<decimal> ConvertCurrency(decimal amount, string currency, DateTime date);
    }
}
