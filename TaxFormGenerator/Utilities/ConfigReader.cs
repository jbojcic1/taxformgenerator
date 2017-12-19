using System;
using System.IO;
using Newtonsoft.Json;

namespace TaxFormGenerator.Utilities
{
    public static class ConfigReader
    {
        public static T ReadFromFile<T>(string filePath)
        {
            using (StreamReader file = File.OpenText(filePath))
            {
                JsonSerializer serializer = new JsonSerializer();
                return (T)serializer.Deserialize(file, typeof(T));
            }
        }
    }
}
