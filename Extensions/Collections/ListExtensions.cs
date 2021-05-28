#region import

using System;
using System.Collections.Generic;
using Unity.Mathematics;

#endregion

namespace Plugins.UnityMonstackCore.Extensions.Collections
{
    public static class ListExtensions
    {
        public static string ToDebugString<T>(this List<T> value)
        {
            var stringValues = new string[value.Count];
            for (var i = 0; i < value.Count; i++) stringValues[i] = value[i].ToString();

            return "[" + string.Join(", ", stringValues) + "]";
        }

        public static bool IsEmpty<T>(this List<T> value)
        {
            return value.Count == 0;
        }

        public static float3 Sum<TSource>(this IEnumerable<TSource> source, Func<TSource, float3> selector)
        {
            var sum = float3.zero;
            foreach (var entry in source)
                sum += selector.Invoke(entry);
            return sum;
        }

        public static float4 Sum<TSource>(this IEnumerable<TSource> source, Func<TSource, float4> selector)
        {
            var sum = float4.zero;
            foreach (var entry in source)
                sum += selector.Invoke(entry);
            return sum;
        }
    }
}