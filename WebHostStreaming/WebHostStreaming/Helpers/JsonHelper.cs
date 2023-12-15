using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace WebHostStreaming.Helpers
{
    public class JsonHelper
    {
        public static T DeserializeFromFile<T>(string filePath) where T : class
        {
            try
            {
                T deserializedObject = null;

                using (StreamReader sw = new StreamReader(filePath))
                {
                    var jsonSerializer = new Newtonsoft.Json.JsonSerializer();
                    deserializedObject = jsonSerializer.Deserialize(sw, typeof(T)) as T;
                }

                return deserializedObject;
            }
            catch (Exception)
            {
                return default(T);
            }
        }

        public static async Task<T> DeserializeFromFileAsync<T>(string filePath) where T : class
        {
            return await Task.Run(() =>
            {
                T deserializedObject = null;

                using (StreamReader sw = new StreamReader(filePath))
                {
                    var jsonSerializer = new Newtonsoft.Json.JsonSerializer();
                    deserializedObject = jsonSerializer.Deserialize(sw, typeof(T)) as T;
                }

                return deserializedObject;
            });
          
        }

        public static async void SerializeToFileAsync(string filePath, object obj)
        {
            await Task.Run(() =>
            {
                using (StreamWriter sw = new StreamWriter(filePath))
                {
                    var jsonSerializer = new Newtonsoft.Json.JsonSerializer();
                    jsonSerializer.Serialize(sw, obj);
                }
            });
        }
    }
}
