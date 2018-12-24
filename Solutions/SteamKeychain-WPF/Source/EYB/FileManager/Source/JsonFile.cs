using Newtonsoft.Json;
using System.IO;

namespace EYB.FileManager
{
    public class JsonFile
    {
        public string FullPath { get; }
        public IEncryptionProvider EncryptionProvider { get; set; }

        public JsonFile(SaveLocation saveLocation, string relativePath)
        {
            FullPath = Files.GetPath(saveLocation, relativePath);
        }

        /// <summary>
        /// Deserialize a Json string into the provided type
        /// </summary>
        public static T Deserialize<T>(string json, JsonSerializerSettings settings = null)
        {
            return (T)JsonConvert.DeserializeObject(json, typeof(T), settings);
        }

        /// <summary>
        /// Serialize an object into a json string
        /// </summary>
        public static string Serialize<T>(T element, JsonSerializerSettings settings = null)
        {
            return JsonConvert.SerializeObject(element, typeof(T), settings);
        }

        /// <summary>
        /// Serialize an object into a json string
        /// </summary>
        public static string Serialize<T>(T element, bool indent)
        {
            return JsonConvert.SerializeObject(element, typeof(T), indent ? Formatting.Indented : Formatting.None, null);
        }

        public void Save<T>(T data, bool indent = false)
        {
            Files.WriteText(FullPath, Serialize(data, indent), EncryptionProvider);
        }

        public T Load<T>(T defaultValue = default)
        {
            return File.Exists(FullPath) ? Deserialize<T>(Files.ReadText(FullPath, EncryptionProvider)) : defaultValue;
        }
    }
}
