using System;

namespace Plugins.UnityMonstackCore.DependencyInjections
{
    [AttributeUsage(AttributeTargets.Constructor)]
    public class AutowiredAttribute : Attribute
    {
    }
}