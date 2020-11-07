using System;

namespace Plugins.Shared.UnityMonstackCore.DependencyInjections
{
    [AttributeUsage(AttributeTargets.Constructor)]
    public class AutowiredAttribute : Attribute
    {
    }
}