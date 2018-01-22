using System;

namespace TaxFormGenerator.Utilities
{
    public static class JOPPDHelper
    {
        public static string GetJOPPDNumber(DateTime date)
        {
            return $"{date.ToString("yy")}{date.DayOfYear.ToString().PadLeft(3, '0')}";
        }
    }
}
