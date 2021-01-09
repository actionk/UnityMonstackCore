using System;
using UnityEngine;

namespace Plugins.Shared.UnityMonstackCore.Utils
{
    [Serializable]
    public struct RangeFloat
    {
        public float min;
        public float max;

        public float Get(float t)
        {
            return Mathf.Lerp(min, max, t);
        }
    }
}