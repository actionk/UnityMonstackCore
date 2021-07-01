using System;
using Plugins.Shared.UnityMonstackCore.Attributes;
using Plugins.Shared.UnityMonstackCore.Utils;
using Unity.Mathematics;

namespace Plugins.Shared.UnityMonstackCore.FunctionBlocks.Modifiers.Float
{
    [Serializable, Identifier("REMAP")]
    public class RemapFloatModifier : FloatModifier
    {
        public RangeFloat from;
        public RangeFloat to;

        public override float Modify(float value)
        {
            return math.remap(from.min, from.max, to.min, to.max, value);
        }
    }
}