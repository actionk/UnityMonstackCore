using Unity.Mathematics;
using UnityEngine;

namespace Plugins.UnityMonstackCore.Extensions
{
    public static class BoundsExtensions
    {
        public static float2x2 ToFloat2x2(this Bounds bounds)
        {
            return new float2x2(
                new float2(bounds.min.x, bounds.min.z),
                new float2(bounds.size.x, bounds.size.z)
            );
        }
    }
}