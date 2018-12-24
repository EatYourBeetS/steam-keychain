using System.IO;

namespace EYB.FileManager
{
    public class TextFile
    {
        public string FullPath { get; }
        public IEncryptionProvider EncryptionProvider { get; set; }

        public TextFile(SaveLocation saveLocation, string relativePath)
        {
            FullPath = Files.GetPath(saveLocation, relativePath);
        }

        public void Save(string content)
        {
            Files.WriteText(FullPath, content, EncryptionProvider);
        }

        public string Load(string defaultValue = null)
        {
            return File.Exists(FullPath) ? Files.ReadText(FullPath, EncryptionProvider) : defaultValue;
        }
    }
}
