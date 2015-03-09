using System;
using System.IO;
using System.Text;
using System.Xml.Serialization;

namespace Nepholo.Model
{
    class Helper
    {
        public static T DeserializeFromXmlFile<T>(string file)
        {
            try
            {
                StreamReader streamReader = new StreamReader(file, Encoding.UTF8);
            
                return DeserializeFromXml<T>(streamReader.ReadToEnd());
            }
            catch (FileNotFoundException)
            {
                return default(T);
            }
        }

        public static void SerializeToXmlFile<T>(string file, T data)
        {
            System.IO.File.WriteAllText(file, SerializeToXml<T>(data));
        }

        public static string SerializeToXml<T>(T toSerialize)
        {
            var serializer = new XmlSerializer(toSerialize.GetType());
            using (var textWriter = new StringWriter())
            {
                serializer.Serialize(textWriter, toSerialize);
                return textWriter.ToString();
            }
        }

        public static T DeserializeFromXml<T>(string toDeserialize)
        {
            try
            {
                var deserializer = new XmlSerializer(typeof (T));
                using (var textReader = new StringReader(toDeserialize))
                {
                    return (T) deserializer.Deserialize(textReader);
                }
            }
            catch (InvalidOperationException invalidOperationException)
            {
                throw new Exception("PlaylistSharp cannot deserialize your xml. Maybe you want to check it first.", invalidOperationException);
            }
        }
    }
}
