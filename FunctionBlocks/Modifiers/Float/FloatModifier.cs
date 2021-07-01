using Sirenix.OdinInspector;
using Sirenix.Serialization;

namespace Plugins.Shared.UnityMonstackCore.FunctionBlocks.Modifiers.Float
{
    public abstract class FloatModifier
    {
        [OdinSerialize, PropertyOrder(-1)]
        public bool enabled = true;

        public abstract float Modify(float value);
    }
}