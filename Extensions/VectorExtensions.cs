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
            return new float2(v.x,  v.y);
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

        public static float2 ToFloat2(this float3 v)
        {
            return new float2(v.x, v.z);
        }

        public static int2 ToInt2(this float2 v)
        {
            return new int2((int) math.floor(v.x), (int) math.floor(v.y));
        }

        public static int2 ToInt2(this float3 v)
        {
            return new int2((int) v.x, (int) v.z);
        }
    }
}