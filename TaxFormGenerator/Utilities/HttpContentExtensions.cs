using System;
using System.Globalization;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace TaxFormGenerator.Utilities
{
    public static class HttpContentExtensions
    {
        public static async Task<T> ReadAsJsonAsync<T>(this HttpContent content, CultureInfo culture = null)
        {
            culture = culture ?? CultureInfo.CurrentCulture;
            string json = await content.ReadAsStringAsync();
            T value = JsonConvert.DeserializeObject<T>(json, new JsonSerializerSettings { Culture = culture });
            return value;
        }
    }
}
