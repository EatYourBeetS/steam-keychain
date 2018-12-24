using System;
using System.Collections.Generic;
using System.IO;

namespace EYB.FileManager
{
    public enum SaveLocation
    {
        /// <summary>
        /// Do not specify any root path
        /// </summary>
        Absolute = 0,

        ///// <summary>
        ///// 'Username/Appdata/Local/UnityEditor/Shared/' (or the OS equivalent) to be used while in editor, shared across all projects
        ///// </summary>
        //EditorPath,

        ///// <summary>
        ///// 'Username/Appdata/Local/UnityEditor/{ProjectGUID}/' (or the OS equivalent) to be used while in editor
        ///// </summary>
        //ProjectPath,

        ///// <summary>
        ///// 'Username/Appdata/LocalLow/{CompanyName}/{ProjectName}/' (or the OS equivalent)
        ///// </summary>
        //ApplicationPath,

        /// <summary>
        /// The path to the application local directory
        /// </summary>
        LocalPath = 4,
    }

    public static class Files
    {
        private static IEncryptionProvider _encryption = new EncryptionProvider("8DwGaaghLe9X6SikWIy9YMjo65DdJt67", "Bx95QAIcAVWupYO8");

        /// <summary>
        /// Do not specify any root path
        /// </summary>
        public static readonly string None = "";

        /// <summary>
        /// The path to the application local directory
        /// </summary>
        public static readonly string LocalPath = AppDomain.CurrentDomain.BaseDirectory;

        // This has to happen after all paths are initialized
        private static readonly Dictionary<SaveLocation, string> _saveLocations = new Dictionary<SaveLocation, string>()
        {
            { SaveLocation.Absolute, "" },
            { SaveLocation.LocalPath, LocalPath }
        };

        /// <summary>
        /// The Encryption provider that will be used to encrypt and decrypt saved data, it cannot be set to null and it uses <see cref="EncryptionProvider"/> by default
        /// </summary>
        public static IEncryptionProvider DefaultEncryptionProvider
        {
            get
            {
                return _encryption;
            }
            set
            {
                _encryption = value ?? throw new ArgumentNullException("Default Encryption Provider cannot be set to null");
            }
        }

        public static void AppendText(SaveLocation saveLocation, string relativePath, string content)
        {
            var path = SystemPath.Combine(_saveLocations[saveLocation], relativePath).AbsolutePath;

            Directory.CreateDirectory(Path.GetDirectoryName(path));
            File.AppendAllText(path, content);
        }

        public static string ReadText(string path, IEncryptionProvider encryptionProvider)
        {
            var text = File.ReadAllText(path);
            return encryptionProvider != null ? _encryption.Decrypt(text) : text;
        }

        public static void WriteText(string path, string content, IEncryptionProvider encryptionProvider)
        {
            if (encryptionProvider != null)
            {
                content = _encryption.Encrypt(content);
            }

            Directory.CreateDirectory(Path.GetDirectoryName(path));
            File.WriteAllText(path, content);
        }

        public static string GetPath(SaveLocation saveLocation, string relativePath)
        {
            return saveLocation != SaveLocation.Absolute
                ? SystemPath.Combine(_saveLocations[saveLocation], relativePath).AbsolutePath
                : SystemPath.Create(relativePath).AbsolutePath;
        }
    }
}