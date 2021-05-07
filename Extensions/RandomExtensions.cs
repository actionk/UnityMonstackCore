using System;
using System.Linq;

namespace Plugins.Shared.UnityMonstackCore.Extensions
{
    public static class RandomExtension
    {
        /// <summary>
        /// Returns a random floating-point number that is greater than or equal to minValue, and less than maxValue.
        /// </summary>
        /// <param name="rand">A random number generator.</param>
        /// <param name="minValue">The inclusive lower bound of the random number returned.</param>
        /// <param name="maxValue">The exclusive upper bound of the random number returned. maxValue must be greater than or equal to minValue.</param>
        /// <returns>A double-precision floating point number that is greater than or equal to minValue, and less than maxValue.</returns>
        public static double NextDouble(this Random rand, double minValue, double maxValue)
        {
            if (rand is null)
                throw new ArgumentNullException(nameof(rand));
            if (minValue > maxValue)
                throw new ArgumentOutOfRangeException(nameof(minValue), minValue, $"'{nameof(minValue)}' must be smaller than or equal to {nameof(maxValue)}.");
            return (maxValue - minValue) * rand.NextDouble() + minValue;
        }

        /// <summary>
        /// Returns a random floating-point number that is greater than or equal to 0.0f, and less than 1.0f.
        /// </summary>
        /// <param name="rand">A random number generator.</param>
        /// <returns>A single-precision floating point number that is greater than or equal to 0.0f, and less than 1.0f.</returns>
        public static float NextFloat(this Random rand)
        {
            if (rand is null)
                throw new ArgumentNullException(nameof(rand));
            return (float)rand.NextDouble();
        }

        /// <summary>
        /// Returns a random floating-point number that is greater than or equal to minValue, and less than maxValue.
        /// </summary>
        /// <param name="rand">A random number generator.</param>
        /// <param name="minValue">The inclusive lower bound of the random number returned.</param>
        /// <param name="maxValue">The exclusive upper bound of the random number returned. maxValue must be greater than or equal to minValue.</param>
        /// <returns>A single-precision floating point number that is greater than or equal to minValue, and less than maxValue.</returns>
        public static float NextFloat(this Random rand, float minValue, float maxValue)
        {
            if (rand is null)
                throw new ArgumentNullException(nameof(rand));
            if (minValue > maxValue)
                throw new ArgumentOutOfRangeException(nameof(minValue), minValue, $"'{nameof(minValue)}' must be smaller than or equal to {nameof(maxValue)}.");
            return (float)rand.NextDouble(minValue, maxValue);
        }

        /// <summary>
        /// Returns a random decimal number that is greater than or equal to 0.0m, and less than 1.0m.
        /// </summary>
        /// <param name="rand">A random number generator.</param>
        /// <returns>A decimal number that is greater than or equal to 0.0m, and less than 1.0m.</returns>
        public static decimal NextDecimal(this Random rand)
        {
            if (rand is null)
                throw new ArgumentNullException(nameof(rand));
            var d = Enumerable.Range(0, 29).Select(x => rand.Next(10).ToString());
            var result = decimal.Parse($"0.{string.Join(string.Empty, d)}");
            return result / 1.000000000000000000000000000000000m;
        }

        /// <summary>
        /// Returns a random decimal number that is greater than or equal to minValue, and less than maxValue.
        /// </summary>
        /// <param name="rand">A random number generator.</param>
        /// <param name="minValue">The inclusive lower bound of the random number returned.</param>
        /// <param name="maxValue">The exclusive upper bound of the random number returned. maxValue must be greater than or equal to minValue.</param>
        /// <returns>A decimal number that is greater than or equal to minValue, and less than maxValue.</returns>
        public static decimal NextDecimal(this Random rand, decimal minValue, decimal maxValue)
        {
            if (rand is null)
                throw new ArgumentNullException(nameof(rand));
            if (minValue > maxValue)
                throw new ArgumentOutOfRangeException(nameof(minValue), minValue, $"'{nameof(minValue)}' must be smaller than or equal to {nameof(maxValue)}.");
            return (maxValue - minValue) * rand.NextDecimal() + minValue;
        }

        /// <summary>
        /// Returns a random byte.
        /// </summary>
        /// <param name="rand">A random number generator.</param>
        /// <returns>A 8-bit unsigned integer that is greater than or equal to 0 and less than MaxValue.</returns>
        public static byte NextByte(this Random rand)
        {
            if (rand is null)
                throw new ArgumentNullException(nameof(rand));
            return (byte)rand.Next(byte.MaxValue);
        }

        /// <summary>
        /// Returns a random byte that is less than the specified maximum.
        /// </summary>
        /// <param name="rand">A random number generator.</param>
        /// <param name="maxValue">The exclusive upper bound of the random number returned. maxValue must be greater than or equal to 0.</param>
        /// <returns>A 8-bit unsigned integer that is greater than or equal to 0 and less than maxValue; that is, the range of return values ordinarily inclueds 0 but not maxValue. However, if maxValue equals 0, maxValue is return.</returns>
        public static byte NextByte(this Random rand, byte maxValue)
        {
            if (rand is null)
                throw new ArgumentNullException(nameof(rand));
            return (byte)rand.Next(maxValue);
        }

        /// <summary>
        /// Returns a random byte that is within a specified range.
        /// </summary>
        /// <param name="rand">A random number generator.</param>
        /// <param name="minValue">The inclusive lower bound of the random number returned.</param>
        /// <param name="maxValue">The exclusive upper bound of the random number returned. maxValue must be greater than or equal to minValue.</param>
        /// <returns>A 8-bit unsigned integer greater than or equal to minValue and less than maxValue; that is, the range of return values includes minValue but not maxValue. If minValue equals maxValue, minValue is returned.</returns>
        public static byte NextByte(this Random rand, byte minValue, byte maxValue)
        {
            if (rand is null)
                throw new ArgumentNullException(nameof(rand));
            if (minValue > maxValue)
                throw new ArgumentOutOfRangeException(nameof(minValue), minValue, $"'{nameof(minValue)}' must be smaller than or equal to {nameof(maxValue)}.");
            return (byte)rand.Next(minValue, maxValue);
        }

        /// <summary>
        /// Returns a non-negative random sbyte.
        /// </summary>
        /// <param name="rand">A random number generator.</param>
        /// <returns>A 8-bit signed integer that is greater than or equal to 0 and less than MaxValue.</returns>
        public static sbyte NextSByte(this Random rand)
        {
            if (rand is null)
                throw new ArgumentNullException(nameof(rand));
            return (sbyte)rand.Next(sbyte.MaxValue);
        }

        /// <summary>
        /// Returns a non-negative random sbyte that is less than the specified maximum.
        /// </summary>
        /// <param name="rand">A random number generator.</param>
        /// <param name="maxValue">The exclusive upper bound of the random number returned. maxValue must be greater than or equal to 0.</param>
        /// <returns>A 8-bit signed integer that is greater than or equal to 0 and less than maxValue; that is, the range of return values ordinarily inclueds 0 but not maxValue. However, if maxValue equals 0, maxValue is return.</returns>
        public static sbyte NextSByte(this Random rand, sbyte maxValue)
        {
            if (rand is null)
                throw new ArgumentNullException(nameof(rand));
            if (maxValue < 0)
                throw new ArgumentOutOfRangeException(nameof(maxValue), maxValue, $"'{nameof(maxValue)}' must be greater than 0.");
            return (sbyte)rand.Next(maxValue);
        }

        /// <summary>
        /// Returns a random sbyte that is within a specified range.
        /// </summary>
        /// <param name="rand">A random number generator.</param>
        /// <param name="minValue">The inclusive lower bound of the random number returned.</param>
        /// <param name="maxValue">The exclusive upper bound of the random number returned. maxValue must be greater than or equal to minValue.</param>
        /// <returns>A 8-bit signed integer greater than or equal to minValue and less than maxValue; that is, the range of return values includes minValue but not maxValue. If minValue equals maxValue, minValue is returned.</returns>
        public static sbyte NextSByte(this Random rand, sbyte minValue, sbyte maxValue)
        {
            if (rand is null)
                throw new ArgumentNullException(nameof(rand));
            if (minValue > maxValue)
                throw new ArgumentOutOfRangeException(nameof(minValue), minValue, $"'{nameof(minValue)}' must be smaller than or equal to {nameof(maxValue)}.");
            return (sbyte)rand.Next(minValue, maxValue);
        }

        /// <summary>
        /// Returns a non-negative random short.
        /// </summary>
        /// <param name="rand">A random number generator.</param>
        /// <returns>A 16-bit signed integer that is greater than or equal to 0 and less than MaxValue.</returns>
        public static short NextShort(this Random rand)
        {
            if (rand is null)
                throw new ArgumentNullException(nameof(rand));
            return (short)rand.Next(short.MaxValue);
        }

        /// <summary>
        /// Returns a non-negative random short that is less than the specified maximum.
        /// </summary>
        /// <param name="rand">A random number generator.</param>
        /// <param name="maxValue">The exclusive upper bound of the random number returned. maxValue must be greater than or equal to 0.</param>
        /// <returns>A 16-bit signed integer that is greater than or equal to 0 and less than maxValue; that is, the range of return values ordinarily inclueds 0 but not maxValue. However, if maxValue equals 0, maxValue is return.</returns>
        public static short NextShort(this Random rand, short maxValue)
        {
            if (rand is null)
                throw new ArgumentNullException(nameof(rand));
            if (maxValue < 0)
                throw new ArgumentOutOfRangeException(nameof(maxValue), maxValue, $"'{nameof(maxValue)}' must be greater than 0.");
            return (short)rand.Next(maxValue);
        }

        /// <summary>
        /// Returns a random short that is within a specified range.
        /// </summary>
        /// <param name="rand">A random number generator.</param>
        /// <param name="minValue">The inclusive lower bound of the random number returned.</param>
        /// <param name="maxValue">The exclusive upper bound of the random number returned. maxValue must be greater than or equal to minValue.</param>
        /// <returns>A 16-bit signed integer greater than or equal to minValue and less than maxValue; that is, the range of return values includes minValue but not maxValue. If minValue equals maxValue, minValue is returned.</returns>
        public static short NextShort(this Random rand, short minValue, short maxValue)
        {
            if (rand is null)
                throw new ArgumentNullException(nameof(rand));
            if (minValue > maxValue)
                throw new ArgumentOutOfRangeException(nameof(minValue), minValue, $"'{nameof(minValue)}' must be smaller than or equal to {nameof(maxValue)}.");
            return (short)rand.Next(minValue, maxValue);
        }

        /// <summary>
        /// Returns a random ushort.
        /// </summary>
        /// <param name="rand">A random number generator.</param>
        /// <returns>A 16-bit unsigned integer that is greater than or equal to 0 and less than MaxValue.</returns>
        public static ushort NextUShort(this Random rand)
        {
            if (rand is null)
                throw new ArgumentNullException(nameof(rand));
            return (ushort)rand.Next(ushort.MaxValue);
        }

        /// <summary>
        /// Returns a random ushort that is less than the specified maximum.
        /// </summary>
        /// <param name="rand">A random number generator.</param>
        /// <param name="maxValue">The exclusive upper bound of the random number returned. maxValue must be greater than or equal to 0.</param>
        /// <returns>A 16-bit unsigned integer that is greater than or equal to 0 and less than maxValue; that is, the range of return values ordinarily inclueds 0 but not maxValue. However, if maxValue equals 0, maxValue is return.</returns>
        public static ushort NextUShort(this Random rand, ushort maxValue)
        {
            if (rand is null)
                throw new ArgumentNullException(nameof(rand));
            return (ushort)rand.Next(maxValue);
        }

        /// <summary>
        /// Returns a random ushort that is within a specified range.
        /// </summary>
        /// <param name="rand">A random number generator.</param>
        /// <param name="minValue">The inclusive lower bound of the random number returned.</param>
        /// <param name="maxValue">The exclusive upper bound of the random number returned. maxValue must be greater than or equal to minValue.</param>
        /// <returns>A 16-bit unsigned integer greater than or equal to minValue and less than maxValue; that is, the range of return values includes minValue but not maxValue. If minValue equals maxValue, minValue is returned.</returns>
        public static ushort NextUShort(this Random rand, ushort minValue, ushort maxValue)
        {
            if (rand is null)
                throw new ArgumentNullException(nameof(rand));
            if (minValue > maxValue)
                throw new ArgumentOutOfRangeException(nameof(minValue), minValue, $"'{nameof(minValue)}' must be smaller than or equal to {nameof(maxValue)}.");
            return (ushort)rand.Next(minValue, maxValue);
        }

        /// <summary>
        /// Returns a random uint.
        /// </summary>
        /// <param name="rand">A random number generator.</param>
        /// <returns>A 32-bit unsigned integer that is greater than or equal to 0 and less than MaxValue.</returns>
        public static uint NextUInt(this Random rand)
        {
            if (rand is null)
                throw new ArgumentNullException(nameof(rand));
            byte[] buffer = new byte[4];
            rand.NextBytes(buffer);
            return BitConverter.ToUInt32(buffer, 0);
        }

        /// <summary>
        /// Returns a random uint that is less than the specified maximum.
        /// </summary>
        /// <param name="rand">A random number generator.</param>
        /// <param name="maxValue">The exclusive upper bound of the random number returned. maxValue must be greater than or equal to 0.</param>
        /// <returns>A 32-bit unsigned integer that is greater than or equal to 0 and less than maxValue; that is, the range of return values ordinarily inclueds 0 but not maxValue. However, if maxValue equals 0, maxValue is return.</returns>
        public static uint NextUInt(this Random rand, uint maxValue)
        {
            return rand.NextUInt(uint.MinValue, maxValue);
        }

        /// <summary>
        /// Returns a random uint that is within a specified range.
        /// </summary>
        /// <param name="rand">A random number generator.</param>
        /// <param name="minValue">The inclusive lower bound of the random number returned.</param>
        /// <param name="maxValue">The exclusive upper bound of the random number returned. maxValue must be greater than or equal to minValue.</param>
        /// <returns>A 32-bit unsigned integer greater than or equal to minValue and less than maxValue; that is, the range of return values includes minValue but not maxValue. If minValue equals maxValue, minValue is returned.</returns>
        public static uint NextUInt(this Random rand, uint minValue, uint maxValue)
        {
            if (rand is null)
                throw new ArgumentNullException(nameof(rand));
            if (minValue > maxValue)
                throw new ArgumentOutOfRangeException(nameof(minValue), minValue, $"'{nameof(minValue)}' must be smaller than or equal to {nameof(maxValue)}.");
            else if (minValue == maxValue)
                return minValue;
            uint range = maxValue - minValue;
            uint bias = uint.MaxValue - uint.MaxValue % range;
            byte[] buffer = new byte[4];
            uint result;
            do
            {
                result = rand.NextUInt();
            } while (result >= bias);
            return result % range + minValue;
        }

        /// <summary>
        /// Returns a non-negative random long.
        /// </summary>
        /// <param name="rand">A random number generator.</param>
        /// <returns>A 64-bit signed integer that is greater than or equal to 0 and less than MaxValue.</returns>
        public static long NextLong(this Random rand)
        {
            return (long)rand.NextULong((ulong)long.MaxValue);
        }

        /// <summary>
        /// Returns a non-negative random long that is less than the specified maximum.
        /// </summary>
        /// <param name="rand">A random number generator.</param>
        /// <param name="maxValue">The exclusive upper bound of the random number returned. maxValue must be greater than or equal to 0.</param>
        /// <returns>A 64-bit signed integer that is greater than or equal to 0 and less than maxValue; that is, the range of return values ordinarily inclueds 0 but not maxValue. However, if maxValue equals 0, maxValue is return.</returns>
        public static long NextLong(this Random rand, long maxValue)
        {
            if (maxValue < 0)
                throw new ArgumentOutOfRangeException(nameof(maxValue), maxValue, $"'{nameof(maxValue)}' must be greater than 0.");
            return (long)rand.NextULong((ulong)maxValue);
        }

        /// <summary>
        /// Returns a random long that is within a specified range.
        /// </summary>
        /// <param name="rand">A random number generator.</param>
        /// <param name="minValue">The inclusive lower bound of the random number returned.</param>
        /// <param name="maxValue">The exclusive upper bound of the random number returned. maxValue must be greater than or equal to minValue.</param>
        /// <returns>A 64-bit signed integer greater than or equal to minValue and less than maxValue; that is, the range of return values includes minValue but not maxValue. If minValue equals maxValue, minValue is returned.</returns>
        public static long NextLong(this Random rand, long minValue, long maxValue)
        {
            if (rand is null)
                throw new ArgumentNullException(nameof(rand));
            if (minValue > maxValue)
                throw new ArgumentOutOfRangeException(nameof(minValue), minValue, $"'{nameof(minValue)}' must be smaller than or equal to {nameof(maxValue)}.");
            else if (minValue == maxValue)
                return minValue;
            ulong umin = minValue < 0 ?
                (ulong)(minValue - long.MinValue) :
                (ulong)minValue + (ulong)long.MaxValue + 1;
            ulong umax = maxValue < 0 ?
                (ulong)(maxValue - long.MinValue) :
                (ulong)maxValue + (ulong)long.MaxValue + 1;
            ulong result = rand.NextULong(umin, umax);
            return result >= (ulong)long.MaxValue + 1 ?
                (long)(result - (ulong)long.MaxValue) - 1 :
                long.MaxValue + (long)result;
        }

        /// <summary>
        /// Returns a random ulong.
        /// </summary>
        /// <param name="rand">A random number generator.</param>
        /// <returns>A 64-bit unsigned integer that is greater than or equal to 0 and less than MaxValue.</returns>
        public static ulong NextULong(this Random rand)
        {
            if (rand is null)
                throw new ArgumentNullException(nameof(rand));
            byte[] buffer = new byte[8];
            rand.NextBytes(buffer);
            return BitConverter.ToUInt64(buffer, 0);
        }

        /// <summary>
        /// Returns a random ulong that is less than the specified maximum.
        /// </summary>
        /// <param name="rand">A random number generator.</param>
        /// <param name="maxValue">The exclusive upper bound of the random number returned. maxValue must be greater than or equal to 0.</param>
        /// <returns>A 64-bit unsigned integer that is greater than or equal to 0 and less than maxValue; that is, the range of return values ordinarily inclueds 0 but not maxValue. However, if maxValue equals 0, maxValue is return.</returns>
        public static ulong NextULong(this Random rand, ulong maxValue)
        {
            return rand.NextULong(ulong.MinValue, maxValue);
        }

        /// <summary>
        /// Returns a random ulong that is within a specified range.
        /// </summary>
        /// <param name="rand">A random number generator.</param>
        /// <param name="minValue">The inclusive lower bound of the random number returned.</param>
        /// <param name="maxValue">The exclusive upper bound of the random number returned. maxValue must be greater than or equal to minValue.</param>
        /// <returns>A 64-bit unsigned integer greater than or equal to minValue and less than maxValue; that is, the range of return values includes minValue but not maxValue. If minValue equals maxValue, minValue is returned.</returns>
        public static ulong NextULong(this Random rand, ulong minValue, ulong maxValue)
        {
            if (rand is null)
                throw new ArgumentNullException(nameof(rand));
            if (minValue > maxValue)
                throw new ArgumentOutOfRangeException(nameof(minValue), minValue, $"'{nameof(minValue)}' must be smaller than or equal to {nameof(maxValue)}.");
            else if (minValue == maxValue)
                return minValue;
            ulong range = maxValue - minValue;
            ulong bias = ulong.MaxValue - ulong.MaxValue % range;
            ulong result;
            do
            {
                result = rand.NextULong();
            } while (result >= bias);
            return result % range + minValue;
        }
    }
}