using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
using UnityEngine;

namespace Plugins.Shared.UnityMonstackCore.FunctionBlocks
{
    [Serializable]
    public class FunctionBlock<T> where T : class
    {
        [OnValueChanged(nameof(OnTypeChanged))]
        [ValueDropdown(nameof(GetTypes))]
        public string type = "";

        [SerializeReference, ShowInInspector, InlineProperty, HideReferenceObjectPicker, HideLabel, ShowIf(nameof(IsDataShown))]
        public T data;

        private bool IsDataShown => data != null;
        protected virtual IEnumerable<ValueDropdownItem> GetTypes => FunctionBlocksManager.Instance.GetValueDropdownByType<T>();

        public FunctionBlock()
        {
#if UNITY_EDITOR
            var functionBlockAttribute = typeof(T).GetCustomAttribute<FunctionBlockAttribute>();
            if (functionBlockAttribute != null && functionBlockAttribute.DefaultValue != null)
                type = functionBlockAttribute.DefaultValue;
#endif

            OnTypeChanged();
        }

        private void OnTypeChanged()
        {
            if (type.IsNullOrWhitespace())
            {
                data = null;
                return;
            }

            var typeOfBlock = FunctionBlocksManager.Instance.GetBlockTypesByType<T>()[type];
            if (data != null && typeOfBlock == data.GetType())
                return;

            if (typeOfBlock != null)
                data = (T) Activator.CreateInstance(typeOfBlock);
            else
                data = null;
        }
    }
}