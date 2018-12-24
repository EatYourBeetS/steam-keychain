using System;
using System.IO;
using System.Reflection;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace EYB.FileManager
{
    public class BinaryFile
    {
        public static readonly BinaryFormatter DefaultFormatter = new BinaryFormatter() { Binder = new BinaryBinder() };

        public string FullPath { get; }
        public BinaryFormatter Formatter { get; }

        public BinaryFile(SaveLocation saveLocation, string relativePath)
        {
            FullPath = Files.GetPath(saveLocation, relativePath);
            Formatter = DefaultFormatter;
        }

        public void Save(object data)
        {
            Directory.CreateDirectory(Path.GetDirectoryName(FullPath));

            using (FileStream stream = File.OpenWrite(FullPath))
            {
                Formatter.Serialize(stream, data);
            }
        }

        public T Load<T>(T defaultValue = default)
        {
            if (File.Exists(FullPath))
            {
                try
                {
                    using (FileStream stream = File.OpenRead(FullPath))
                    {
                        return (T)Formatter.Deserialize(stream);
                    }
                }
                catch
                {
                    return defaultValue;
                }
            }
            else
            {
                return defaultValue;
            }
        }

        public class BinaryBinder : SerializationBinder
        {
            public override Type BindToType(string assemblyName, string typeName)
            {
                return Type.GetType(string.Format("{0}, {1}", typeName, assemblyName)) ?? Type.GetType(string.Format("{0}, {1}", typeName, Assembly.GetExecutingAssembly().FullName));
            }
        }
    }
}
