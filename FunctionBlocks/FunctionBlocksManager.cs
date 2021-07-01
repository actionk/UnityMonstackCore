using System;
using System.Collections.Generic;
using System.Linq;
using Plugins.Shared.UnityMonstackCore.Attributes;
using Plugins.Shared.UnityMonstackCore.Utils;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
using UnityEngine;

namespace Plugins.Shared.UnityMonstackCore.FunctionBlocks
{
    public class FunctionBlocksManager
    {
        private const string EMPTY_STRING = "";

        private readonly Dictionary<string, List<ValueDropdownItem>> m_valueDropdownCache = new Dictionary<string, List<ValueDropdownItem>>();
        private readonly Dictionary<Type, Dictionary<string, Type>> m_cache = new Dictionary<Type, Dictionary<string, Type>>();

        public Dictionary<string, Type> GetBlockTypesByType<T>()
        {
            return GetBlockTypesByType(typeof(T));
        }

        public Dictionary<string, Type> GetBlockTypesByType(Type targetType)
        {
            if (m_cache.TryGetValue(targetType, out Dictionary<string, Type> output))
                return output;

            var cache = ReflectionUtils
                .GetAllTypesWithInterface(targetType)
                .Where(x => !x.IsAbstract && x.GetCustomAttribute<IdentifierAttribute>() != null)
                .ToDictionary(x => x.GetCustomAttribute<IdentifierAttribute>().Id);
            m_cache[targetType] = cache;
            return cache;
        }

        public List<ValueDropdownItem> GetValueDropdownByType<T>()
        {
            return GetValueDropdownByType(typeof(T));
        }

        public List<ValueDropdownItem> GetValueDropdownByType<T>(string filterName = default, Func<KeyValuePair<string, Type>, bool> predicate = null)
        {
            return GetValueDropdownByType(typeof(T), filterName, predicate);
        }

        public List<ValueDropdownItem> GetValueDropdownByType(Type targetType, string filterName = default, Func<KeyValuePair<string, Type>, bool> predicate = null)
        {
            var name = targetType.Name + filterName;

            if (m_valueDropdownCache.TryGetValue(name, out List<ValueDropdownItem> output))
                return output;

            var cache = GetBlockTypesByType(targetType).AsEnumerable();
            if (predicate != null)
                cache = cache.Where(predicate);

            var cacheList = cache
                .Select(x => new ValueDropdownItem(x.Key, x.Value != null ? x.Key : ""))
                .OrderBy(x => x.Text)
                .ToList();

            var functionBlockAttribute = targetType.GetCustomAttribute<FunctionBlockAttribute>();
            if (functionBlockAttribute != null)
            {
                if (functionBlockAttribute.EmptyValueDropdownKey != null)
                    cacheList.Insert(0, new ValueDropdownItem(functionBlockAttribute.EmptyValueDropdownKey, EMPTY_STRING));
            }

            m_valueDropdownCache[name] = cacheList;
            return cacheList;
        }

        private static FunctionBlocksManager m_instance;

        public static FunctionBlocksManager Instance => m_instance ??= new FunctionBlocksManager();

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        public static void Reset()
        {
            m_instance = new FunctionBlocksManager();
        }
    }
}