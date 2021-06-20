using System;

namespace Plugins.Shared.UnityMonstackCore.FunctionBlocks
{
    public class FunctionBlockAttribute : Attribute
    {
        public string EmptyValueDropdownKey { get; set; }
        public string DefaultValue { get; set; }
    }
}