﻿using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Plugins.Shared.UnityMonstackCore.Utils
{
    [Serializable, InlineProperty]
    public struct RangeFloat
    {
        [HorizontalGroup("Range", LabelWidth = 35)]
        public float min;

        [HorizontalGroup("Range", LabelWidth = 35)]
        public float max;

        public RangeFloat(int min, int max)
        {
            this.min = min;
            this.max = max;
        }

        public float Evaluate(float t)
        {
            return Mathf.Lerp(min, max, t);
        }
    }
}