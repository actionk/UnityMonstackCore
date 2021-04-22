#region import

using System;
using System.Collections.Generic;

#endregion

namespace Plugins.UnityMonstackCore.Extensions.Collections
{
    public static class DictionaryExtensions
    {
        public static TValue GetValueOrDefault<TKey, TValue>
        (this IDictionary<TKey, TValue> dictionary,
            TKey key,
            TValue defaultValue)
        {
            TValue value;
            return dictionary.TryGetValue(key, out value) ? value : defaultValue;
        }

        public static TValue GetValueOrCompute<TKey, TValue>
        (this IDictionary<TKey, TValue> dictionary,
            TKey key,
            Func<TKey, TValue> func)
        {
            TValue result;

            if (!dictionary.TryGetValue(key, out result))
            {
                result = func(key);
                dictionary.Add(key, result);
            }

            return result;
        }

        public static TValue GetValueOrCompute<TKey, TValue>
        (this IDictionary<TKey, TValue> dictionary,
            TKey key,
            Func<TValue> func)
        {
            TValue result;

            if (!dictionary.TryGetValue(key, out result))
            {
                result = func();
                dictionary.Add(key, result);
            }

            return result;
        }

        public static void ForEachKey<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, Action<TKey> action)
        {
            foreach (var entry in dictionary) action.Invoke(entry.Key);
        }

        public static void ForEachValue<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, Action<TValue> action)
        {
            foreach (var entry in dictionary) action.Invoke(entry.Value);
        }

        public static TValue ComputeIfAbsent<TKey, TValue>(this Dictionary<TKey, TValue> dictionary, TKey key,
            Func<TKey, TValue> func)
        {
            TValue result;

            if (!dictionary.TryGetValue(key, out result))
            {
                result = func(key);
                dictionary.Add(key, result);
            }

            return result;
        }

        public static TValue SetOrComputeIfPresent<TKey, TValue>(this Dictionary<TKey, TValue> dictionary, TKey key, TValue value, Func<TValue, TValue, TValue> presentCallback)
        {
            TValue result;

            if (!dictionary.TryGetValue(key, out result))
            {
                result = value;
                dictionary.Add(key, result);
            }
            else
            {
                result = presentCallback.Invoke(result, value);
                dictionary[key] = result;
            }

            return result;
        }

        public static bool TryGetValueCasted<TOutput>(this Dictionary<string, object> dictionary, string key, out TOutput output)
        {
            output = default;

            if (!dictionary.TryGetValue(key, out var valueRead))
                return false;

            if (!(valueRead is TOutput valueParsed))
                return false;

            output = valueParsed;
            return true;
        }

        public static TOutput GetValueCastedOrDefault<TOutput>(this Dictionary<string, object> dictionary, string key, TOutput defaultValue)
        {
            if (dictionary.TryGetValueCasted(key, out TOutput output))
                return output;

            return defaultValue;
        }
    }
}