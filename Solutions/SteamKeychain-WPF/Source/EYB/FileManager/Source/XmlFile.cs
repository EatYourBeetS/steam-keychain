using System.IO;
using System.Xml.Serialization;

namespace EYB.FileManager
{
    public class XmlFile
    {
        public string FullPath { get; }
        public IEncryptionProvider EncryptionProvider { get; set; }

        public XmlFile(SaveLocation saveLocation, string relativePath)
        {
            FullPath = Files.GetPath(saveLocation, relativePath);
        }

        public void Save<T>(T data)
        {
            Directory.CreateDirectory(Path.GetDirectoryName(FullPath));

            var serializer = new XmlSerializer(typeof(T));
            var namespaces = new XmlSerializerNamespaces();
            namespaces.Add("", "");

            using (StreamWriter writer = File.CreateText(FullPath))
            {
                serializer.Serialize(writer, data, namespaces);
            }
        }

        public T Load<T>(T defaultValue = default)
        {
            if (File.Exists(FullPath))
            {
                using (FileStream stream = File.OpenRead(FullPath))
                {
                    var serializer = new XmlSerializer(typeof(T));
                    var namespaces = new XmlSerializerNamespaces();
                    namespaces.Add("", "");

                    return (T)serializer.Deserialize(stream);
                }
            }
            else
            {
                return defaultValue;
            }
        }
    }
}
