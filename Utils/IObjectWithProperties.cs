using System.Collections.Generic;

namespace Plugins.Shared.UnityMonstackCore.Utils
{
    public interface IObjectWithProperties<TKey, TProperty>
    {
        List<TProperty> Properties { get; }

        TProperty GetProperty(TKey key);
        bool RemoveProperty(TKey key);
        T AddProperty<T>(T property) where T : TProperty;
        bool TryGetProperty<T>(out T value) where T : TProperty;
        void SetProperty<T>(T property) where T : TProperty;
    }
}