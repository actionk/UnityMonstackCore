using System;

namespace Plugins.Shared.UnityMonstackCore.DependencyInjections
{
    [AttributeUsage(AttributeTargets.Class)]
    public class InstantiableAttribute : Attribute
    {
        public InstantiableAttribute(Type basicType)
        {
            BasicType = basicType;
        }

        public Type BasicType { get; }
    }
}