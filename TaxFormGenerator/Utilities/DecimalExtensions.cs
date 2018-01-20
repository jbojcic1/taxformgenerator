using System;

namespace TaxFormGenerator.Utilities
{
    public static class DecimalExtensions
    {
        public static decimal Round(this decimal numberToRound, int decimalPlaces = 2)
        {
            return Math.Round(numberToRound, decimalPlaces, MidpointRounding.AwayFromZero);
        }
    }
}
