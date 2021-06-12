using System;
using System.Collections.Generic;
using System.Linq;
using Plugins.Shared.UnityMonstackCore.Extensions;
using Unity.Mathematics;
using UnityEngine;

namespace Plugins.UnityMonstackCore.Extensions
{
    public static class ListExtensions
    {
        public static int FirstIndexOf<T>(this IEnumerable<T> source, Predicate<T> predicate)
        {
            var index = 0;

            using (IEnumerator<T> enumerator = source.GetEnumerator())
            {
                while (enumerator.MoveNext())
                {
                    if (predicate.Invoke(enumerator.Current))
                        return index;

                    index++;
                }
            }

            return -1;
        }

        public static TSource MinBy<TSource, TKey>(this IEnumerable<TSource> source,
            Func<TSource, TKey> selector)
        {
            return source.MinBy(selector, null);
        }

        public static TSource Random<TSource>(this IEnumerable<TSource> sources)
        {
            if (!sources.Any())
                return default;

            return sources.ElementAt(UnityEngine.Random.Range(0, sources.Count()));
        }

        public static IEnumerable<TSource> Shuffle<TSource>(this IEnumerable<TSource> source) => source.OrderBy(a => Guid.NewGuid());

        public static TSource MinBy<TSource, TKey>(this IEnumerable<TSource> source,
            Func<TSource, TKey> selector, IComparer<TKey> comparer)
        {
            if (source == null) throw new ArgumentNullException("source");
            if (selector == null) throw new ArgumentNullException("selector");
            if (comparer == null) comparer = Comparer<TKey>.Default;

            using (var sourceIterator = source.GetEnumerator())
            {
                if (!sourceIterator.MoveNext())
                {
                    throw new InvalidOperationException("Sequence contains no elements");
                }

                var min = sourceIterator.Current;
                var minKey = selector(min);
                while (sourceIterator.MoveNext())
                {
                    var candidate = sourceIterator.Current;
                    var candidateProjected = selector(candidate);
                    if (comparer.Compare(candidateProjected, minKey) < 0)
                    {
                        min = candidate;
                        minKey = candidateProjected;
                    }
                }

                return min;
            }
        }

        public static TSource MaxBy<TSource, TKey>(this IEnumerable<TSource> source,
            Func<TSource, TKey> selector)
        {
            return source.MaxBy(selector, null);
        }

        public static TSource MaxBy<TSource, TKey>(this IEnumerable<TSource> source,
            Func<TSource, TKey> selector, IComparer<TKey> comparer)
        {
            if (source == null) throw new ArgumentNullException("source");
            if (selector == null) throw new ArgumentNullException("selector");
            if (comparer == null) comparer = Comparer<TKey>.Default;

            using (var sourceIterator = source.GetEnumerator())
            {
                if (!sourceIterator.MoveNext())
                {
                    throw new InvalidOperationException("Sequence contains no elements");
                }

                var max = sourceIterator.Current;
                var maxKey = selector(max);
                while (sourceIterator.MoveNext())
                {
                    var candidate = sourceIterator.Current;
                    var candidateProjected = selector(candidate);
                    if (comparer.Compare(candidateProjected, maxKey) > 0)
                    {
                        max = candidate;
                        maxKey = candidateProjected;
                    }
                }

                return max;
            }
        }

        public static void RemoveAll<T>(this List<T> value, Func<T, bool> callback)
        {
            for (var i = 0; i < value.Count; i++)
            {
                if (callback.Invoke(value[i]))
                    value.RemoveAt(i--);
            }
        }

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