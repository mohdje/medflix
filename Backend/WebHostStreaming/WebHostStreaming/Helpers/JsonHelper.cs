
using System;
using System.IO;
using System.Text.Json;

namespace WebHostStreaming.Helpers
{
    public class JsonHelper
    {
        public static T DeserializeFromFile<T>(string filePath) where T : class
        {
            try
            {
                var json = File.ReadAllText(filePath);

                return JsonSerializer.Deserialize<T>(json);
            }
            catch (Exception)
            {
                return default(T);
            }
        }

        public static void SerializeToFile(string filePath, object obj)
        {
            string jsonString = JsonSerializer.Serialize(obj);
            File.WriteAllText(filePath, jsonString);
        }
    }
}
