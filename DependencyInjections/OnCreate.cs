#region import

using System;

#endregion

namespace Plugins.Shared.UnityMonstackCore.DependencyInjections
{
    [AttributeUsage(AttributeTargets.Method)]
    public class OnCreate : Attribute
    {
    }
}