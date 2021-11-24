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
        public static void SaveAsXmlFormat<T>(T list, string fileName)
        {
            using  (StreamWriter sw = File.CreateText(fileName))
            {
                JsonSerializer serializer = new();
                serializer.Serialize(sw, list);
            }
        }
        public static T ReadAsXmlFormat<T>(string fileName)
        {
            T result = default;
            using (StreamReader sr = File.OpenText(fileName))
            {
                JsonSerializer serializer = new();
                result = (T)serializer.Deserialize(sr, typeof(T));
            }
            return result;
        }
    }
}
