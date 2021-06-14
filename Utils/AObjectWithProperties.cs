using System.Collections.Generic;
using System.Linq;

namespace Plugins.Shared.UnityMonstackCore.Utils
{
    public abstract class AObjectWithProperties<TKey, TProperty> : IObjectWithProperties<TKey, TProperty>
    {
        protected Dictionary<TKey, TProperty> properties;

        protected abstract TKey GetKeyByData<T>(T data) where T : TProperty;
        public List<TProperty> Properties => properties?.Values.ToList();

        public TProperty GetProperty(TKey key)
        {
            if (properties == null) return default;
            return properties[key];
        }

        public bool RemoveProperty(TKey key)
        {
            if (properties == null) return default;
            return properties.Remove(key);
        }

        public T AddProperty<T>(T property) where T : TProperty
        {
            properties ??= new Dictionary<TKey, TProperty>();
            properties[GetKeyByData(property)] = property;
            return property;
        }

        public T AddProperty<T>() where T : TProperty, new()
        {
            properties ??= new Dictionary<TKey, TProperty>();
            var property = new T();
            properties[GetKeyByData(property)] = property;
            return property;
        }

        public bool TryGetProperty<T>(out T value) where T : TProperty
        {
            value = default;

            if (properties == null)
                return false;

            if (!properties.TryGetValue(GetKeyByData(value), out var existingValue))
                return false;

            value = (T) existingValue;
            return true;
        }

        public void SetProperty<T>(T property) where T : TProperty
        {
            AddProperty(property);
        }
    }
}