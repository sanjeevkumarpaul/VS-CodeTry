using System;
using System.IO;
using System.Xml.Serialization;
using Newtonsoft.Json.Serialization;
using Newtonsoft.Json;

namespace Utilities.Helpers
{
    public static class Serializer
    {
        static Serializer() { }

        public static string Serialize<T>(T type)
        {
            XmlSerializer engine = new XmlSerializer(typeof(T));
            string data = string.Empty;

            using (StringWriter writer = new StringWriter())
            {
                engine.Serialize(writer, type);
                writer.Dispose();
                data = writer.ToString();
            }

            return data;
        }


        public static T DeSerialize<T>(string xml) where T : new()
        {
            XmlSerializer engine = new XmlSerializer(typeof(T));
            T type = default(T);

            using (TextReader reader = new StringReader(xml))
            {
                type = (T)engine.Deserialize(reader);
                reader.Dispose();
            }

            return type;
        }

        public static string JScriptSerialize<T>(T obj)
        {
            try
            {
                // Serializing an object into JSON results                 
                return JsonConvert.SerializeObject( obj );

            }
            catch (Exception ex)
            {
                throw new Exception("Error DeSerializing JSON results into an object.  " + ex);
            }
        }

        public static T JScriptDeSerialize<T>(string results)
        {
            try
            {
                // deserializing JSON results into an object
                return JsonConvert.DeserializeObject<T>( results );                
            }
            catch (Exception ex)
            {
                throw new Exception("Error DeSerializing JSON results into an object.  " + ex);
            }
        }
    }
}