using System;
using Plugins.Shared.UnityMonstackCore.Attributes;
using Plugins.Shared.UnityMonstackCore.Utils;

namespace Plugins.Shared.UnityMonstackCore.FunctionBlocks.Conditions.Float
{
    [Serializable, Identifier("RANGE")]
    public class RangeFloatCondition : IFloatCondition
    {
        public RangeFloat range;

        public bool Validate(float value)
        {
            return range.Contains(value);
        }
    }
}