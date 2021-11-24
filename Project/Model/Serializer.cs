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
        //public static void SaveAsXmlFormat<T>(T list, string fileName)
        //{
        //    XmlSerializer xmlFormat = new XmlSerializer(typeof(T));
        //    using (Stream fStream = new FileStream(fileName,
        //    FileMode.Create, FileAccess.Write, FileShare.None))
        //    {
        //        xmlFormat.Serialize(fStream, list);
        //    }
        //}
        //public static T ReadAsXmlFormat<T>(string fileName)
        //{
        //    XmlSerializer xmlFormat = new XmlSerializer(typeof(T));
        //    using (Stream fStream = new FileStream(fileName, FileMode.Open))
        //    {
        //        T obj = default;
        //        obj = (T)xmlFormat.Deserialize(fStream);
        //        return obj;
        //    }
        //}
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
            //T result = default;
            //using (StreamReader sr = File.OpenText(fileName))
            //{
            //    string json = sr.ReadToEnd();
            //    JsonSerializer serializer = new();
            //    //result = (T)serializer.Deserialize(sr, typeof(T));
            //    result = JsonConvert.DeserializeObject<T>(json);
            //}
            //return result;
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
