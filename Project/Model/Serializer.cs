using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
namespace Project.Model
{
    public static class Serializer
    {
        public static void SaveAsJsonFormat<T>(T list, string fileName)
        {
            using  (StreamWriter sw = File.CreateText(fileName))
            {
                Newtonsoft.Json.JsonSerializer serializer = new Newtonsoft.Json.JsonSerializer();
                serializer.Converters.Add(new Newtonsoft.Json.Converters.JavaScriptDateTimeConverter());
                serializer.NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore;
                serializer.TypeNameHandling = Newtonsoft.Json.TypeNameHandling.Auto;
                serializer.Formatting = Newtonsoft.Json.Formatting.Indented;
                serializer.Serialize(sw, list);
            }
        }
        public static T ReadAsJsonFormat<T>(string fileName)
        {
            T obj = Newtonsoft.Json.JsonConvert.DeserializeObject<T>(File.ReadAllText(fileName), new Newtonsoft.Json.JsonSerializerSettings
            {
                TypeNameHandling = Newtonsoft.Json.TypeNameHandling.Auto,
                NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore,
            });
            return obj;
        }

        public static void SaveAdList<T>(List<T> list, string fileName)
        {
            using (StreamWriter sr = File.CreateText(fileName))
            {
                JsonSerializer serializer = new();
                serializer.Serialize(sr, typeof(List<T>));
            }
        }
    }
}
