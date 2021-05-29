#region import

using System;
using System.Collections.Generic;
using Plugins.Shared.UnityMonstackCore.Extensions;
using Unity.Mathematics;
using UnityEngine;

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

        public static Color SumColor<TSource>(this IEnumerable<TSource> source, Func<TSource, Color> selector)
        {
            var sum = float4.zero;
            foreach (var entry in source)
                sum += selector.Invoke(entry).ToFloat4();
            return sum.ToColor();
        }

        public static float4 SumFloat4<TSource>(this IEnumerable<TSource> source, Func<TSource, float4> selector)
        {
            var sum = float4.zero;
            foreach (var entry in source)
                sum += selector.Invoke(entry);
            return sum;
        }
    }
}