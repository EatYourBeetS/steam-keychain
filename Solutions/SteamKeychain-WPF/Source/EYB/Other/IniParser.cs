using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace EYB
{
    public class IniParser
    {
        private string _filePath = null;
        private List<Token> _tokens = new List<Token>();
        private StringBuilder _builder = null;

        public Property GetProperty(string propertyName, bool searchInSections = false)
        {
            var prop = _tokens.OfType<Property>().FirstOrDefault(p => p.Name == propertyName);
            if (prop == null && searchInSections)
            {
                prop = _tokens.OfType<Section>().SelectMany(s => s.Tokens).OfType<Property>().FirstOrDefault(p => p.Name == propertyName);
            }
            return prop;
        }

        public Section GetSection(string sectionName)
        {
            return _tokens.OfType<Section>().FirstOrDefault(t => t.Name == sectionName);
        }

        public IniParser(string filePath)
        {
            if (File.Exists(filePath))
            {
                var lines = File.ReadAllLines(filePath);
                for (var i = 0; i < lines.Length; i++)
                {
                    _tokens.Add(Parse(ref i, lines));
                }
            }
            else
            {
                File.Create(filePath);
            }
        }

        public void SaveChanges()
        {
            _builder = new StringBuilder();
            foreach (var token in _tokens)
            {
                token.Write(_builder);
            }
            File.WriteAllText(_filePath, _builder.ToString());
        }

        public Token Parse(ref int i, string[] lines)
        {
            if (i >= lines.Length)
            {
                return null;
            }

            var line = lines[i];

            var section = Section.TryParse(line);
            if (section != null)
            {
                Token token;
                do
                {
                    i += 1;
                    token = Parse(ref i, lines);
                }
                while (section.TryAdd(token));

                return section;
            }

            return Section.TryParse(line) ?? Property.TryParse(line) ?? (Token)new Error(line);
        }

        #region Tokens

        public abstract class Token
        {
            public abstract void Write(StringBuilder writer);
        }

        public class Section : Token
        {
            private List<Token> _tokens = new List<Token>();
            private string _name = null;

            public string Name { get { return _name; } set { _name = value; } }
            public List<Token> Tokens { get { return _tokens; } }

            public static Section TryParse(string line)
            {
                if (string.IsNullOrEmpty(line) || line.Length < 3)
                {
                    return null;
                }

                return line.StartsWith("[") && line.EndsWith("]")
                    ? new Section()
                    {
                        _name = line.Substring(1, line.Length - 1),
                        _tokens = new List<Token>(),
                    }
                    : null;
            }

            public bool TryAdd(Token token)
            {
                if (token == null || token is Section)
                {
                    return false;
                }
                else
                {
                    _tokens.Add(token);

                    return true;
                }
            }

            public override void Write(StringBuilder writer)
            {
                writer.Append("[").Append(_name).AppendLine("]");

                foreach (var token in _tokens)
                {
                    token.Write(writer);
                }
            }
        }

        public class Comment : Token
        {
            private string _text = null;

            public string Text { get { return _text; } set { _text = value; } }

            public static Comment TryRead(string line)
            {
                if (line == null)
                {
                    return null;
                }

                line = line.Trim();
                return line == "" || line.StartsWith(";")
                    ? new Comment()
                    {
                        _text = line.Length > 1 ? line.Substring(1) : "",
                    }
                    : null;
            }

            public override void Write(StringBuilder writer)
            {
                writer.AppendLine(_text != "" ? ";" + _text : "");
            }
        }

        public class Property : Token
        {
            private bool _isString = false;
            private string _name = "";
            private string _value = "";

            public string Name { get { return _name; } set { _name = value; } }

            public T GetValue<T>() where T : IConvertible
            {
                return _value.Convert(default(T));
            }

            public void SetValue<T>(T value) where T : IConvertible
            {
                _isString = value is string;
                _value = value.ToString();
            }

            public static Property TryParse(string line)
            {
                if (line == null)
                {
                    return null;
                }

                var items = line.Split(new[] { '=' }, count: 2);
                if (items.Length == 2)
                {
                    var prop = new Property()
                    {
                        _name = items[0].Trim()
                    };

                    var value = items[1].Trim();
                    if (value.StartsWith("\"") && value.EndsWith("\""))
                    {
                        prop._value = value.Trim(new char[] { '"' });
                        prop._isString = true;
                    }
                    else
                    {
                        prop._value = value;
                        prop._isString = false;
                    }

                    return prop;
                }

                return null;
            }

            public override void Write(StringBuilder writer)
            {
                if (_isString)
                {
                    writer.Append(_name).Append(" = \"").Append(_value).AppendLine("\"");
                }
                else
                {
                    writer.Append(_name).Append(" = ").AppendLine(_value);
                }
            }
        }

        public class Error : Token
        {
            private string _text = "";

            public string Text { get { return _text; } set { _text = value; } }

            public override void Write(StringBuilder writer)
            {

            }

            public Error(string line)
            {
                _text = line;
            }
        }

        #endregion
    }
}