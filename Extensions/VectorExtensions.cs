using Unity.Mathematics;
using UnityEngine;

namespace Plugins.UnityMonstackCore.Extensions
{
    public static class VectorExtensions
    {
        public static int2 ToInt2(this Vector3 v)
        {
            return new int2((int) math.floor(v.x), (int) math.floor(v.z));
        }

        public static int2 ToClosestInt2(this Vector3 v)
        {
            return new int2((int) math.round(v.x), (int) math.round(v.z));
        }

        public static int2 ToClosestInt2(this float3 v)
        {
            return new int2((int) math.round(v.x), (int) math.round(v.z));
        }

        public static float2 ToFloat2(this Vector3 v)
        {
            return new float2(v.x, v.z);
        }

        public static Vector2 ToVector2(this Vector3 v)
        {
            return new Vector2(v.x, v.z);
        }

        public static Vector3 ToVector3(this Vector2 v)
        {
            return new Vector3(v.x, 0, v.y);
        }

        public static float3 ToFloat3(this Vector3 v)
        {
            return new float3(v.x, v.y, v.z);
        }

        public static float2 ToFloat2(this Vector2 v)
        {
            return new float2(v.x, v.y);
        }

        public static float3 ToFloat3(this Vector2 v)
        {
            return new float3(v.x, 0.0f, v.y);
        }

        public static int2 ToInt2(this Vector2 v)
        {
            return new int2((int) math.floor(v.x), (int) math.floor(v.y));
        }

        public static int3 ToInt3(this Vector3 v)
        {
            return new int3((int) math.floor(v.x), (int) math.floor(v.y), (int) math.floor(v.z));
        }

        public static float3 ToFloat3(this int3 v)
        {
            return new float3(v.x, v.y, v.z);
        }

        public static float3 ToFloat3(this int2 v, float y = 0)
        {
            return new float3(v.x, y, v.y);
        }

        public static float2 ToFloat2(this int2 v)
        {
            return new float2(v.x, v.y);
        }

        public static Vector2 ToVector2(this int2 v)
        {
            return new Vector2(v.x, v.y);
        }

        /// <summary>
        /// Converting world 2D coordinates to grid coordinates
        /// </summary>
        /// <param name="v">Original coordinates</param>
        /// <param name="size">Size of the building / object to rely on</param>
        /// <param name="y">Y-coordinate (if needed)</param>
        /// <param name="inverse">If true, it will apply 0.5f to even cells (not odd)</param>
        /// <returns></returns>
        public static Vector3 ToWorldGridPosition(this int2 v, int2 size, float y = 0.0f, bool inverse = false)
        {
            return new Vector3(v.x + (size.x % 2 == (inverse ? 0 : 1) ? 0.5f : 0), y, v.y + (size.y % 2 == (inverse ? 0 : 1) ? 0.5f : 0));
        }

        public static Vector3 ToVector3(this int2 v)
        {
            return new Vector3(v.x, 0, v.y);
        }

        public static float3 ToFloat3(this float2 v, float y = 0)
        {
            return new float3(v.x, y, v.y);
        }

        public static Vector3 ToVector3(this float2 v, float y = 0)
        {
            return new Vector3(v.x, y, v.y);
        }

        public static Vector2 ToVector2(this float2 v)
        {
            return new Vector3(v.x, v.y);
        }

        public static Vector3 ToVector3(this float3 v)
        {
            return new Vector3(v.x, v.y, v.z);
        }

        public static Vector2 ToVector2(this float3 v)
        {
            return new Vector3(v.x, v.z);
        }

        public static float2 ToFloat2(this float3 v)
        {
            return new float2(v.x, v.z);
        }

        public static float3 WithY(this float3 v, float y)
        {
            return new float3(v.x, y, v.z);
        }

        public static Vector3 WithY(this Vector3 v, float y)
        {
            return new Vector3(v.x, y, v.z);
        }

        public static int2 ToInt2(this float2 v)
        {
            return new int2((int) math.floor(v.x), (int) math.floor(v.y));
        }

        public static int2 ToInt2(this float3 v)
        {
            return new int2((int) math.floor(v.x), (int) math.floor(v.z));
        }
    }
}