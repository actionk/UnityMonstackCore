#region import

using System;

#endregion

namespace Plugins.UnityMonstackCore.DependencyInjections
{
    [AttributeUsage(AttributeTargets.Method)]
    public class OnCreate : Attribute
    {
    }
}