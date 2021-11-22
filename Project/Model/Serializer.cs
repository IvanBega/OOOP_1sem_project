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
        public static void SaveAsXmlFormat<T>(T list, string fileName)
        {
            XmlSerializer xmlFormat = new XmlSerializer(typeof(T));
            using (Stream fStream = new FileStream(fileName,
            FileMode.Create, FileAccess.Write, FileShare.None))
            {
                xmlFormat.Serialize(fStream, list);
            }
        }
        public static T ReadAsXmlFormat<T>(string fileName)
        {
            XmlSerializer xmlFormat = new XmlSerializer(typeof(T));
            using (Stream fStream = new FileStream(fileName, FileMode.Open))
            {
                T obj = default;
                obj = (T)xmlFormat.Deserialize(fStream);
                return obj;
            }
        }
    }
}
