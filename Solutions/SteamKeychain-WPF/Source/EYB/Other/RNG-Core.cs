using System;

namespace EYB
{
    public class RNG
    {
        private uint _seed = 0;
        private uint _x = 0;
        private uint _y = 0;
        private uint _z = 0;

        public static readonly RNG Instance = new RNG();

        public RNG()
        {
            Initialize((uint)Environment.TickCount);
        }

        public RNG(int seed)
        {
            Initialize((uint)seed);
        }

        public RNG(uint seed)
        {
            Initialize(seed);
        }

        private void Initialize(uint seed)
        {
            unchecked
            {
                _seed = seed;
                _x = (seed + 0868640911) % 30000;
                _y = (_x + seed + 1429104923) % 30000;
                _z = (_y + _x + seed + 4124354783) % 30000;
            }
        }

#pragma warning disable RCS1215 // Expression is always equal to true/false.
        /// <summary>
        /// Return a double from 0 to 1(not included)
        /// </summary>
        public double NextDouble()
        {
            _x = (171 * (_x % 177)) - (2 * (_x / 177));
            if (_x < 0)
            {
                _x += 30269;
            }
            _y = (172 * (_y % 176)) - (35 * (_y / 176));
            if (_y < 0)
            {
                _y += 30307;
            }
            _z = (170 * (_z % 178)) - (63 * (_z / 178));
            if (_z < 0)
            {
                _z += 30323;
            }

            var r = ((_x * 1.0) / 30269d) + ((_y * 1.0) / 30307d) + ((_z * 1.0) / 30323d);
            return r - Math.Truncate(r);
        }
#pragma warning restore RCS1215 // Expression is always equal to true/false.

        /// <summary>
        /// Return a double from 0 to max(not included)
        /// </summary>
        public double NextDouble(double max)
        {
            if (max <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(max), max, "max must be > 0");
            }

            return NextDouble() * max;
        }

        /// <summary>
        /// Return a double from min to max(not included)
        /// </summary>
        public double NextDouble(double min, double max)
        {
            if (max < min)
            {
                throw new ArgumentOutOfRangeException(nameof(max), max, "max must be > min");
            }

            return min + (NextDouble() * (max - min));
        }

        /// <summary>
        /// Return a random boolean value
        /// </summary>
        public bool NextBool()
        {
            return NextDouble() < 0.5;
        }

        /// <summary>
        /// Return a float from 0 to 1(not included)
        /// </summary>
        public float NextFloat()
        {
            return (float)NextDouble();
        }

        /// <summary>
        /// Return a float from 0 to max(not included)
        /// </summary>
        public float NextFloat(float max)
        {
            if (max <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(max), max, "max must be > 0");
            }

            return (float)NextDouble() * max;
        }

        /// <summary>
        /// Return a float from min to max(not included)
        /// </summary>
        public float NextFloat(float min, float max)
        {
            if (max < min)
            {
                throw new ArgumentOutOfRangeException(nameof(max), max, "max must be > min");
            }

            return min + (float)(NextDouble() * (max - min));
        }

        /// <summary>
        /// Return either -value or +value
        /// </summary>
        public int NextSign(int value = 1)
        {
            return NextBool() ? value : -value;
        }

        /// <summary>
        /// Return a positive integer from 0 to int.MaxValue(not included)
        /// </summary>
        public int NextInt()
        {
            return (int)(NextDouble() * int.MaxValue);
        }

        /// <summary>
        /// Return a positive integer from 0 to max(not included)
        /// </summary>
        public int NextInt(int max)
        {
            if (max <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(max), max, "max must be > 0");
            }

            return (int)(NextDouble() * max);
        }

        /// <summary>
        /// Return an integer from min to max(not included)
        /// </summary>
        public int NextInt(int min, int max)
        {
            if (max < min)
            {
                throw new ArgumentOutOfRangeException(nameof(max), max, "max must be > min");
            }

            return min + (int)(NextDouble() * (max - min));
        }

        /// <summary>
        /// Return a positive integer from 0 to int.MaxValue(not included)
        /// </summary>
        public uint NextUint()
        {
            return (uint)(NextDouble() * uint.MaxValue);
        }

        /// <summary>
        /// Return a positive integer from 0 to max(not included)
        /// </summary>
        public uint NextUint(uint max)
        {
            if (max <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(max), max, "max must be > 0");
            }

            return (uint)(NextDouble() * max);
        }

        /// <summary>
        /// Return a positive integer from min to max(not included)
        /// </summary>
        public uint NextUint(uint min, uint max)
        {
            if (max < min)
            {
                throw new ArgumentOutOfRangeException(nameof(max), max, "max must be > min");
            }

            return min + (uint)(NextDouble() * (max - min));
        }

        /// <summary>
        /// Return an array of random bytes
        /// </summary>
        public byte[] NextBytes(int length)
        {
            var result = new byte[length];
            var temp = NextInt();

            var i = 0;
            while (true)
            {
                temp = NextInt();

                if (i >= length)
                {
                    return result;
                }

                result[i++] = (byte)(temp >> 24);

                if (i >= length)
                {
                    return result;
                }

                result[i++] = (byte)(temp >> 16);

                if (i >= length)
                {
                    return result;
                }

                result[i++] = (byte)(temp >> 8);

                if (i >= length)
                {
                    return result;
                }

                result[i++] = (byte)(temp);
            }
        }

        /// <summary>
        /// Return a random string built as a combinations of the provided characters
        /// </summary>
        public string NextString(int length, string characters)
        {
            var count = characters.Length;

            var stringChars = new char[length];

            for (var i = 0; i < length; i++)
            {
                stringChars[i] = characters[NextInt(count)];
            }

            return new string(stringChars);
        }

        /// <summary>
        /// Return a random alphanumeric string of the specified length
        /// </summary>
        public string NextAlphaNumericString(int length)
        {
            const string chars = "ABCDEFGHIJ" +  // 10
                                 "KLMNOPQRST" +  // 20
                                 "UVWXYZabcd" +  // 30
                                 "efghijklmn" +  // 40
                                 "opqrstuvwx" +  // 50
                                 "yz01234567" +  // 60
                                 "89";           // 62
            const int count = 62;

            var stringChars = new char[length];

            for (var i = 0; i < length; i++)
            {
                stringChars[i] = chars[NextInt(count)];
            }

            return new string(stringChars);
        }
    }
}