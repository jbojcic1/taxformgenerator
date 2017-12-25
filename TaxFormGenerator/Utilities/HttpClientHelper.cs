using System;
using System.Net.Http;
using System.Text;
using Newtonsoft.Json;

namespace TaxFormGenerator.Utilities
{
    public static class HttpClientHelper
    {
        public static HttpContent GetJsonHttpContent(object obj)
        {
            return new StringContent(JsonConvert.SerializeObject(obj), Encoding.UTF8, "application/json");
        }
    }
}
