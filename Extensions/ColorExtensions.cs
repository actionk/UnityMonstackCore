using Unity.Mathematics;
using UnityEngine;

namespace Plugins.Shared.UnityMonstackCore.Extensions
{
    public static class ColorExtensions
    {
        public static Color WithAlpha(this Color color, float alpha)
        {
            return new Color(color.r, color.g, color.b, alpha);
        }

        public static float4 ToFloat4(this Color color)
        {
            return new float4(color.r, color.g, color.b, color.a);
        }

        public static Color ToColor(this float4 color)
        {
            return new Color(color.x, color.y, color.z, color.w);
        }
    }
}