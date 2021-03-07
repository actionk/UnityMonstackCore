using System;
using System.Collections.Generic;
using System.Linq;

namespace Plugins.Shared.UnityMonstackCore.Utils
{
    public class ObjectWithProperties
    {
        private readonly Dictionary<Type, object> m_properties = new Dictionary<Type, object>();

        public List<object> Properties => m_properties.Values.ToList();

        public void AddProperty<T>(T property)
        {
            SetProperty(property);
        }

        public void SetProperty<T>(T property)
        {
            m_properties[typeof(T)] = property;
        }

        public bool RemoveProperty<T>()
        {
            return m_properties.Remove(typeof(T));
        }

        public T GetProperty<T>()
        {
            return (T) m_properties[typeof(T)];
        }

        public bool HasProperty<T>()
        {
            return HasProperty(typeof(T));
        }

        public bool HasProperty(Type type)
        {
            return m_properties.ContainsKey(type);
        }

        public bool TryGetProperty<T>(out T property)
        {
            bool hasProperty = m_properties.TryGetValue(typeof(T), out object objectProperty);
            property = (T) objectProperty;
            return hasProperty;
        }

        public T GetOrCreateProperty<T>(Func<T> createCallback)
        {
            var key = typeof(T);
            if (m_properties.TryGetValue(key, out object value))
                return (T) value;

            var newValue = createCallback.Invoke();
            m_properties[key] = newValue;
            return newValue;
        }

        public T GetOrCreateProperty<T>() where T : new()
        {
            var key = typeof(T);
            if (m_properties.TryGetValue(key, out object value))
                return (T) value;

            var newValue = new T();
            m_properties[key] = newValue;
            return newValue;
        }
    }
}