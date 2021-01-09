using System;
using UnityEngine;

namespace Plugins.Shared.UnityMonstackCore.Utils
{
    [Serializable]
    public struct RangeInt
    {
        public int min;
        public int max;

        public int Get(float t)
        {
            return (int)Mathf.Lerp(min, max, t);
        }
    }
}