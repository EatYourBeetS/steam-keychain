using System.Collections.Generic;
using System.IO;
using System.Text;

namespace EYB
{
    public class SystemPath
    {
        public string AbsolutePath         { get; private set; }
        public string Extension            { get; private set; }
        public string Name                 { get; private set; }
        public string NameWithoutExtension { get; private set; }
        public bool   IsDirectory          { get; private set; }
        public bool   Exists               { get { return IsDirectory ? Directory.Exists(AbsolutePath) : File.Exists(AbsolutePath); } }

        public static SystemPath Create(string absolutePath)
        {
            absolutePath = ValidatePath(absolutePath, true);
            if (absolutePath == null)
            {
                return null;
            }

            FileSystemInfo info;
            if (Directory.Exists(absolutePath))
            {
                absolutePath = absolutePath.TrimEnd('/');

                info = new DirectoryInfo(absolutePath);
            }
            else if (File.Exists(absolutePath))
            {
                info = new FileInfo(absolutePath);
            }
            else
            {
                if (absolutePath.EndsWith("/"))
                {
                    absolutePath = absolutePath.TrimEnd('/');

                    info = new DirectoryInfo(absolutePath);
                }
                else
                {
                    info = new FileInfo(absolutePath);
                }
            }

            return new SystemPath
            {
                AbsolutePath = absolutePath,
                IsDirectory = info is DirectoryInfo,
                Extension = (info is DirectoryInfo) ? "/" : info.Extension,
                Name = info.Name,
                NameWithoutExtension = info.Name.TrimEnd(info.Extension)
            };
        }

        public static SystemPath Combine(params string[] parts)
        {
            return Create(CombinePath(true, parts));
        }

        public static SystemPath Combine(SystemPath root, params string[] parts)
        {
            return root.IsDirectory ? Combine(root.AbsolutePath, CombinePath(false, parts)) : null;
        }

        public SystemPath GetParent()
        {
            var parent = Directory.GetParent(AbsolutePath);
            return parent != null ? Create(parent.FullName) : null;
        }

        public IEnumerable<SystemPath> GetExistingChildren()
        {
            if (IsDirectory)
            {
                foreach (var path in Directory.GetFileSystemEntries(AbsolutePath))
                {
                    var node = Create(path);
                    if (node?.Exists == true)
                    {
                        yield return node;
                    }
                }
            }
        }

        public static string ValidatePath(string path, bool useAbsolutePath = true)
        {
            if (string.IsNullOrEmpty(path) || path.Length < 4)
            {
                return null;
            }

            var sb = new StringBuilder();
            var currentChar = '\0';
            var lastChar = '\0';
            var i = 0;

            if (useAbsolutePath)
            {
                currentChar = path[0];

                if (char.IsLetter(currentChar))
                {
                    sb.Append(char.ToUpper(currentChar));
                }
                else
                {
                    return null;
                }

                currentChar = path[1];
                if (currentChar == ':')
                {
                    sb.Append(':');
                }
                else
                {
                    return null;
                }

                i = 2;
            }

            while (i < path.Length)
            {
                currentChar = path[i];

                if (currentChar == '\\' || currentChar == '/')
                {
                    currentChar = '/';

                    if (lastChar != currentChar)
                    {
                        sb.Append(currentChar);
                    }
                }
                else if (currentChar <= 31 || currentChar == '"' || currentChar == '\b' || currentChar == '\t' || currentChar == '\n' || "<?:*|>".Contains(currentChar.ToString()))
                {
                    return null;
                }
                else
                {
                    sb.Append(currentChar);
                }

                lastChar = currentChar;
                i += 1;
            }

            return sb.ToString();
        }

        public static string CombinePath(bool startWithAbsolute, params string[] parts)
        {
            if (parts == null || parts.Length == 0)
            {
                return null;
            }

            var i = 0;
            var sb = new StringBuilder();
            var currentPart = "";
            var lastPart = "";

            if (startWithAbsolute)
            {
                currentPart = ValidatePath(parts[0], true);
                if (currentPart == null)
                {
                    return null;
                }
                lastPart = currentPart;
                sb.Append(currentPart);
                i = 1;
            }

            while (i < parts.Length)
            {
                currentPart = ValidatePath(parts[i], false);
                if (currentPart == null)
                {
                    return null;
                }

                if (lastPart.EndsWith("/"))
                {
                    if (currentPart.StartsWith("/"))
                    {
                        sb.Append(currentPart.Substring(1));
                    }
                    else
                    {
                        sb.Append(currentPart);
                    }
                }
                else if (currentPart.StartsWith("/"))
                {
                    sb.Append(currentPart);
                }
                else
                {
                    sb.Append("/");
                    sb.Append(currentPart);
                }

                i += 1;
            }

            return sb.ToString();
        }

        public string GetRelativePath(SystemPath root)
        {
            return root == null || AbsolutePath.StartsWith(root.AbsolutePath) == false ? null : AbsolutePath.Substring(root.AbsolutePath.Length);
        }

        public string GetRelativePath(string root)
        {
            return GetRelativePath(Create(root));
        }

        public bool CompareFileContent(SystemPath other)
        {
            if (Exists == false || other.Exists == false)
            {
                return false;
            }

            var file1 = this.AbsolutePath;
            var file2 = other.AbsolutePath;

            if (file1 == file2)
            {
                return true;
            }

            using (var fs1 = new FileStream(file1, FileMode.Open, FileAccess.Read))
            using (var fs2 = new FileStream(file2, FileMode.Open, FileAccess.Read))
            {
                if (fs1.Length != fs2.Length)
                {
                    return false;
                }

                var file1byte = 0;
                var file2byte = 0;

                while ((file1byte == file2byte) && (file1byte != -1))
                {
                    file1byte = fs1.ReadByte();
                    file2byte = fs2.ReadByte();
                }

                return (file1byte - file2byte) == 0;
            }
        }
    }
}