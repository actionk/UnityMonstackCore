using System;
using System.Collections.Generic;
using System.Linq;

namespace Plugins.Shared.UnityMonstackCore.Utils
{
    /// <summary>
    /// Extension to the normal Dictionary. This class can store more than one value for every key. It keeps a List for every Key value.
    /// Calling Add with the same Key and multiple values will store each value under the same Key in the Dictionary. Obtaining the values
    /// for a Key will return the List with the Values of the Key. 
    /// </summary>
    /// <typeparam name="TKey">The type of the key.</typeparam>
    /// <typeparam name="TValue">The type of the value.</typeparam>
    public class OrderedMultiValueDictionary<TKey, TValue> : Dictionary<TKey, List<TValue>>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="OrderedMultiValueDictionary&lt;TKey, TValue&gt;"/> class.
        /// </summary>
        public OrderedMultiValueDictionary()
            : base()
        {
        }


        /// <summary>
        /// Adds the specified value under the specified key
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="value">The value.</param>
        public void Add(TKey key, TValue value)
        {
            List<TValue> container = null;
            if (!this.TryGetValue(key, out container))
            {
                container = new List<TValue>();
                base.Add(key, container);
            }

            container.Add(value);
        }


        /// <summary>
        /// Adds all values under the specified key
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="value">The value.</param>
        public void AddRange(TKey key, List<TValue> value)
        {
            List<TValue> container = null;
            if (!this.TryGetValue(key, out container))
            {
                container = new List<TValue>();
                base.Add(key, container);
            }

            foreach (var inner in value)
            {
                container.Add(inner);
            }
        }


        /// <summary>
        /// Determines whether this dictionary contains the specified value for the specified key 
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="value">The value.</param>
        /// <returns>true if the value is stored for the specified key in this dictionary, false otherwise</returns>
        public bool ContainsValue(TKey key, TValue value)
        {
            bool toReturn = false;
            List<TValue> values = null;
            if (this.TryGetValue(key, out values))
            {
                toReturn = values.Contains(value);
            }

            return toReturn;
        }


        /// <summary>
        /// Removes the specified value for the specified key. It will leave the key in the dictionary.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="value">The value.</param>
        public bool Remove(TKey key, TValue value)
        {
            List<TValue> container = null;
            if (this.TryGetValue(key, out container))
            {
                container.Remove(value);
                if (container.Count <= 0)
                {
                    this.Remove(key);
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Removes the specified value for the specified key. It will leave the key in the dictionary.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="value">The value.</param>
        public bool RemoveAll(Func<TKey, bool> callback)
        {
            var removed = false;

            var keys = Keys.ToArray();
            foreach (var key in keys)
            {
                if (callback.Invoke(key))
                {
                    Remove(key);
                    removed = true;
                }
            }

            return removed;
        }


        /// <summary>
        /// Merges the specified OrderedMultiValueDictionary into this instance.
        /// </summary>
        /// <param name="toMergeWith">To merge with.</param>
        public void Merge(OrderedMultiValueDictionary<TKey, TValue> toMergeWith)
        {
            if (toMergeWith == null)
            {
                return;
            }

            foreach (KeyValuePair<TKey, List<TValue>> pair in toMergeWith)
            {
                foreach (TValue value in pair.Value)
                {
                    this.Add(pair.Key, value);
                }
            }
        }


        /// <summary>
        /// Gets the values for the key specified. This method is useful if you want to avoid an exception for key value retrieval and you can't use TryGetValue
        /// (e.g. in lambdas)
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="returnEmptySet">if set to true and the key isn't found, an empty List is returned, otherwise, if the key isn't found, null is returned</param>
        /// <returns>
        /// This method will return null (or an empty set if returnEmptySet is true) if the key wasn't found, or
        /// the values if key was found.
        /// </returns>
        public List<TValue> GetValues(TKey key, bool returnEmptySet = false)
        {
            List<TValue> toReturn = null;
            if (!base.TryGetValue(key, out toReturn) && returnEmptySet)
            {
                toReturn = new List<TValue>();
            }

            return toReturn;
        }

        public IEnumerable<TValue> GetAllValues()
        {
            foreach (var List in Values)
            {
                foreach (var value in List)
                {
                    yield return value;
                }
            }
        }

        public List<TValue> GetValues()
        {
            var result = new List<TValue>();
            foreach (var List in Values)
            {
                foreach (var value in List)
                    result.Add(value);
            }

            return result;
        }
    }

    public static class OrderedMultiValueDictionaryExtensions
    {
        public static OrderedMultiValueDictionary<TKey, TValue> ToOrderedMultiValueDictionary<TKey, TValue>(this IEnumerable<TValue> values, Func<TValue, TKey> keySelector)
        {
            var orderedMultiValueDictionary = new OrderedMultiValueDictionary<TKey, TValue>();
            foreach (var entry in values)
                orderedMultiValueDictionary.Add(keySelector.Invoke(entry), entry);
            return orderedMultiValueDictionary;
        }

        public static OrderedMultiValueDictionary<TKey, TValue> ToOrderedMultiValueDictionary<T, TKey, TValue>(this IEnumerable<T> values, Func<T, TKey> keySelector,
            Func<T, TValue> valueSelector)
        {
            var orderedMultiValueDictionary = new OrderedMultiValueDictionary<TKey, TValue>();
            foreach (var entry in values)
            {
                orderedMultiValueDictionary.Add(keySelector.Invoke(entry), valueSelector.Invoke(entry));
            }

            return orderedMultiValueDictionary;
        }

        public static OrderedMultiValueDictionary<TKey, TSelectorValue> ToOrderedMultiValueDictionaryOfType<TKey, TValue, TSelectorValue>(this IEnumerable<TValue> values,
            Func<TValue, TKey> keySelector)
        {
            var orderedMultiValueDictionary = new OrderedMultiValueDictionary<TKey, TSelectorValue>();
            foreach (var entry in values)
            {
                if (entry is TSelectorValue castedValue)
                    orderedMultiValueDictionary.Add(keySelector.Invoke(entry), castedValue);
            }

            return orderedMultiValueDictionary;
        }
    }
}