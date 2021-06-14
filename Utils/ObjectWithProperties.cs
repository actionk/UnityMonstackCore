using System;
using System.Collections.Generic;

namespace Plugins.Shared.UnityMonstackCore.Utils
{
    public class ObjectWithProperties<TProperty> : AObjectWithProperties<Type, TProperty>, IObjectWithPropertiesByType<TProperty>
    {
        protected override Type GetKeyByData<T>(T data)
        {
            return typeof(T);
        }

        public T GetProperty<T>() where T : TProperty
        {
            return (T) GetProperty(typeof(T));
        }

        public bool RemoveProperty<T>() where T : TProperty
        {
            return RemoveProperty(typeof(T));
        }

        public T GetOrCreateProperty<T>() where T : TProperty, new() 
        {
            properties ??= new Dictionary<Type, TProperty>();
            
            var key = typeof(T);
            if (properties.TryGetValue(key, out var property))
                return (T) property;

            var value = new T();
            properties[key] = value;
            return value;
        }
        
        public T GetPropertyOrDefault<T>() where T : TProperty
        {
            if (properties == null) return default;

            var key = typeof(T);
            return properties.ContainsKey(key) ? (T) properties[key] : default;
        }
        
        public T GetOrCreateProperty<T>(Func<T> createCallback) where T : TProperty
        {
            properties ??= new Dictionary<Type, TProperty>();

            var key = typeof(T);
            if (properties.TryGetValue(key, out var property))
                return (T) property;

            var newValue = createCallback.Invoke();
            properties[key] = newValue;
            return newValue;
        }

        public T GetPropertyOrDefault<T>(T defaultValue) where T : TProperty
        {
            if (properties == null)
                return defaultValue;

            var key = typeof(T);
            if (!properties.TryGetValue(key, out var value))
                return defaultValue;

            return (T) value;
        }

        public bool HasProperty<T>() where T : TProperty
        {
            if (properties == null) return false;

            var key = typeof(T);
            return properties.ContainsKey(key);
        }
    }
}