using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml.XPath;

namespace EYB
{
    public static class _ExtensionMethods
    {
        private static RNG _random = new RNG();
        private static readonly XPathNavigator _navigator = new XPathDocument(new StringReader("<r/>")).CreateNavigator();

        /// <summary>
        /// Try to convert a string into a given IConvertible type
        /// </summary>
        public static bool TryConvert<T>(this string value, ref T result) where T : IConvertible
        {
            try
            {
                var type = typeof(T);
                if (type == typeof(string))
                {
                    result = (T)(object)value;
                }
                else if (type.IsEnum)
                {
                    result = (T)Enum.Parse(type, value);
                }
                else
                {
                    result = (T)System.Convert.ChangeType(value, type);
                }

                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Convert a string into a given IConvertible type
        /// </summary>
        public static T Convert<T>(this string value, T fallbackValue) where T : IConvertible
        {
            TryConvert(value, ref fallbackValue);

            return fallbackValue;
        }

        /// <summary>
        /// Convert a string into a given IConvertible type
        /// </summary>
        public static T Convert<T>(this string value) where T : IConvertible
        {
            try
            {
                var type = typeof(T);
                if (type == typeof(string))
                {
                    return (T)(object)value;
                }
                else
                {
                    return type.IsEnum ? (T)Enum.Parse(type, value) : (T)System.Convert.ChangeType(value, type);
                }
            }
            catch (Exception ex)
            {
                throw new ArgumentException("Could not parse '" + value + "' to " + typeof(T), ex);
            }
        }

        /// <summary>
        /// Check if the string is not null or empty ("")
        /// </summary>
        public static bool IsNonEmpty(this string s)
        {
            return string.IsNullOrEmpty(s) == false;
        }

        /// <summary>
        /// Take the first n characters of a string, or the whole string if it's longer than n
        /// </summary>
        public static string UpTo(this string s, int n)
        {
            return s.Substring(0, s.Length > n ? n : s.Length);
        }

        /// <summary>
        /// Ensure a string starts with the given prefix, adding it if it is missing
        /// </summary>
        public static string AddPrefix(this string s, string prefix, StringComparison comparisonType = StringComparison.CurrentCulture)
        {
            return s.StartsWith(prefix, comparisonType) ? s : s + prefix;
        }

        /// <summary>
        /// Ensure a string ends with the given suffix, by adding it if it is missing
        /// </summary>
        public static string AddSuffix(this string s, string suffix, StringComparison comparisonType = StringComparison.CurrentCulture)
        {
            return s.EndsWith(suffix, comparisonType) ? s : s + suffix;
        }

        /// <summary>
        /// Similar to the normal TrimEnd, but it acts on a string rather than a character
        /// </summary>
        public static string TrimEnd(this string s, string toRemove)
        {
            return s.EndsWith(toRemove) ? s.Substring(0, s.Length - toRemove.Length) : s;
        }

        /// <summary>
        /// Continuously call TrimEnd until the string does not end with the characters to remove, e.g: "abcdcdcd".TrimEndRecursively("cd") == "ab"
        /// </summary>
        public static string TrimEndRepeated(this string s, string toRemove)
        {
            while (s.EndsWith(toRemove))
            {
                s = TrimEnd(s, toRemove);
            }

            return s;
        }

        /// <summary>
        /// Turn "sample string" into "Sample String"
        /// </summary>
        public static string ToTitleCase(this string s)
        {
            return CultureInfo.CurrentCulture.TextInfo.ToTitleCase(s.ToLower());
        }

        /// <summary>
        /// Round the date to the nearest multiple of timespan
        /// </summary>
        public static DateTime RoundUp(this DateTime dt, TimeSpan d)
        {
            return new DateTime((dt.Ticks + d.Ticks - 1) / d.Ticks * d.Ticks, dt.Kind);
        }

        /// <summary>
        /// Execute an action for each member of the IEnumerable
        /// </summary>
        public static void ForEach<T>(this IEnumerable<T> source, Action<T> action)
        {
            foreach (var element in source)
            {
                action(element);
            }
        }

        public static T FirstOrDefault<T>(this IEnumerable<T> source, Predicate<T> predicate, T defaultValue)
        {
            foreach (var item in source)
            {
                if (predicate(item))
                {
                    return item;
                }
            }

            return defaultValue;
        }

        public static T ElementAtOrDefault<T>(this IEnumerable<T> source, int index, T defaultValue = default)
        {
            var i = 0;

            foreach (var item in source)
            {
                if (i == index)
                {
                    return item;
                }
                i += 1;
            }

            return defaultValue;
        }

        /// <summary>
        /// Return the index of the first appearance of an item in the IEnumerable
        /// </summary>
        public static int IndexOf<T>(this IEnumerable<T> items, T item)
        {
            var i = 0;
            foreach (var element in items)
            {
                if (element.Equals(item))
                {
                    return i;
                }
                i += 1;
            }

            return -1;
        }

        /// <summary>
        /// Return the index of the first element that satisfies the predicate, or -1 if none
        /// </summary>
        public static int IndexOf<T>(this IEnumerable<T> items, Predicate<T> predicate)
        {
            var i = 0;
            foreach (var element in items)
            {
                if (predicate(element))
                {
                    return i;
                }
                i += 1;
            }

            return -1;
        }

        /// <summary>
        /// Iterates a list in reverse order, more efficient than IEnumerable.Reverse
        /// </summary>
        public static IEnumerable<T> Reverse<T>(this List<T> items)
        {
            for (var i = items.Count - 1; i >= 0; i--)
            {
                yield return items[i];
            }
        }

        /// <summary>
        /// Iterates an array in reverse order, more efficient than IEnumerable.Reverse
        /// </summary>
        public static IEnumerable<T> Reverse<T>(this T[] items)
        {
            for (var i = items.Length - 1; i >= 0; i--)
            {
                yield return items[i];
            }
        }

        /// <summary>
        /// If the ienumerable is null, return an empty IEnumerable instead
        /// </summary>
        public static IEnumerable<T> EmptyIfNull<T>(this IEnumerable<T> items)
        {
            return items ?? Enumerable.Empty<T>();
        }

        /// <summary>
        /// Return true if the IEnumerable is null or contains no element
        /// </summary>
        public static bool IsNullOrEmpty<T>(this IEnumerable<T> items)
        {
            return items == null || items.Any() == false;
        }

        /// <summary>
        /// Return false if the IEnumerable is null or contains no element
        /// </summary>
        public static bool IsNonEmpty<T>(this IEnumerable<T> items)
        {
            return items?.Any() == true;
        }

        /// <summary>
        /// Remove a random element from a list
        /// </summary>
        public static void RemoveRandomElement<T>(this IList<T> items)
        {
            items.RemoveAt(_random.NextInt(0, items.Count));
        }

        /// <summary>
        /// Get a random element from an IEnumerable
        /// </summary>
        public static T Sample<T>(this IEnumerable<T> items)
        {
            return items.ElementAtOrDefault(_random.NextInt(0, items.Count()));
        }

        /// <summary>
        /// Returns a number of random samples from an IEnumerable
        /// </summary>
        public static IEnumerable<T> Samples<T>(this IEnumerable<T> items, int samples, bool allowRepeatedSamples)
        {
            if (allowRepeatedSamples == false)
            {
                var max = items.Count() < samples ? items.Count() : samples;
                foreach (var item in items.Shuffle().Take(max))
                {
                    yield return item;
                }
            }
            else
            {
                for (var i = 0; i < samples; i++)
                {
                    yield return items.Sample();
                }
            }
        }

        /// <summary>
        /// Reorganize the IEnumerable in a random order
        /// </summary>
        public static IOrderedEnumerable<T> Shuffle<T>(this IEnumerable<T> items)
        {
            return items.OrderBy(_ => _random.NextDouble());
        }

        /// <summary>
        /// Evaluate a mathematical expression contained within a string, supporting these symbols [+ - / * % ( )]
        /// </summary>
        public static string EvaluateMathematicalExpression(this string expression)
        {
            return _navigator.Evaluate("number(" + new Regex(@"([\+\-\*])").Replace(expression, " ${1} ").Replace("/", " div ").Replace("%", " mod ") + ")").ToString();
        }

        /// <summary>
        /// Turn a single byte into a bool array of Length == 8
        /// </summary>
        public static bool[] ToBoolArray(this byte b)
        {
            var target = new bool[8];

            // check each bit in the byte. if 1 set to true, if 0 set to false
            for (var i = 0; i < 8; i++)
            {
                target[7 - i] = (b & (1 << i)) != 0;
            }

            return target;
        }

        /// <summary>
        /// Turn a bool array into a single byte, the array must have Length == 8
        /// </summary>
        public static byte ToByte(this bool[] source)
        {
            byte result = 0;

            for (var i = 0; i < source.Length; i++)
            {
                // if the element is 'true' set the bit at that position
                if (source[i])
                {
                    result |= (byte)(1 << (7 - i));
                }
            }

            return result;
        }

        /* Little Endian version

        public static bool[] ToBoolArray(this byte b)
        {
            var target = new bool[8];

            // check each bit in the byte. if 1 set to true, if 0 set to false
            for (var i = 0; i < 8; i++)
            {
                target[i] = (b & (1 << i)) != 0;
            }

            return target;
        }

        public static byte ToByte(this bool[] source)
        {
            byte result = 0;

            for (var i = 0; i < source.Length; i++)
            {
                // if the element is 'true' set the bit at that position
                if (source[7 - i])
                {
                    result |= (byte)(1 << (7 - i));
                }
            }

            return result;
        }

        */

        /// <summary>
        /// Turn a byte array into a bool array, Length will be 8 times greater
        /// </summary>
        [Obsolete("This method may act differently across platforms")]
        public static bool[] ToBoolArray(this byte[] source)
        {
            var target = new bool[source.Length * 8];
            for (var t = 0; t < target.Length / 8; t++)
            {
                for (var i = 0; i < 8; i++)
                {
                    target[(t * 8) + i] = (source[t] & (1 << i)) != 0;
                }
            }

            return target;
        }

        /// <summary>
        /// Turn a bool array into a byte array, with Length reduced to 1/8th
        /// </summary>
        [Obsolete("This method may act differently across platforms")]
        public static byte[] ToByteArray(this bool[] source)
        {
            var target = new byte[source.Length / 8];
            var len = source.Length;
            var bytes = len >> 3;
            if ((len & 0x07) != 0)
            {
                bytes += 1;
            }

            for (var i = 0; i < len; i++)
            {
                if (source[i])
                {
                    target[i >> 3] |= (byte)(1 << (i & 0x07));
                }
            }

            return target;
        }
    }
}