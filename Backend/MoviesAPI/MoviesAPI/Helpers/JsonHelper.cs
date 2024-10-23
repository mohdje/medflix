using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.IO;

namespace MoviesAPI.Helpers
{
    internal static class JsonHelper
    {
        public static string ToJson(object value)
        {
            DefaultContractResolver contractResolver = new DefaultContractResolver
            {
                NamingStrategy = new CamelCaseNamingStrategy()
            };

            return JsonConvert.SerializeObject(value, new JsonSerializerSettings { ContractResolver = contractResolver });
        }

        public static T ToObject<T>(string json) where T : class
        {
            try
            {
                return JsonConvert.DeserializeObject(json, typeof(T)) as T;
            }
            catch (System.Exception ex)
            {
                return null;
            }
            
        }

      
    }
}