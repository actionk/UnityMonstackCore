using System;

namespace Plugins.Shared.UnityMonstackCore.Utils
{
    public interface IObjectWithPropertiesByType<TProperty> : IObjectWithProperties<Type, TProperty>
    {
        T AddProperty<T>() where T : TProperty, new();
        bool RemoveProperty<T>() where T : TProperty;
        T GetProperty<T>() where T : TProperty;
        T GetOrCreateProperty<T>() where T : TProperty, new();
        T GetPropertyOrDefault<T>() where T : TProperty;
        T GetOrCreateProperty<T>(Func<T> createCallback) where T : TProperty;
        T GetPropertyOrDefault<T>(T defaultValue) where T : TProperty;
        bool HasProperty<T>() where T : TProperty;
    }
}