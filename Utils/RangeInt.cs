using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Plugins.Shared.UnityMonstackCore.Utils
{
    [Serializable]
    public struct RangeInt
    {
        [HorizontalGroup("Range"), LabelWidth(100)]
        public int min;

        [HorizontalGroup("Range"), LabelWidth(100)]
        public int max;

        public int Get(float t)
        {
            return (int) Mathf.Lerp(min, max, t);
        }
    }
}