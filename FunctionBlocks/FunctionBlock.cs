using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using Sirenix.Utilities;
using UnityEngine;

namespace Plugins.Shared.UnityMonstackCore.FunctionBlocks
{
    [Serializable, ShowOdinSerializedPropertiesInInspector]
    public class FunctionBlock<T> where T : class
    {
        public static FunctionBlock<T> CreateInstance()
        {
            return new FunctionBlock<T>();
        }

        [OdinSerialize, HideInInspector]
        public bool disableTypeSelection;

        [OdinSerialize, OnValueChanged(nameof(OnTypeChanged)), ValueDropdown(nameof(GetTypes)), HideIf(nameof(disableTypeSelection))]
        public string type = "";

        [NonSerialized, OdinSerialize, ShowInInspector, InlineProperty, HideReferenceObjectPicker, HideLabel, HideIf(nameof(IsEmpty))]
        public T data;

        public bool IsEmpty => data == null;
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

            SetType(type);
        }

        public void SetType(string newType)
        {
            var typeOfBlock = FunctionBlocksManager.Instance.GetBlockTypesByType<T>()[newType];
            if (data != null && typeOfBlock == data.GetType())
                return;

            type = newType;

            if (typeOfBlock != null)
                data = (T) Activator.CreateInstance(typeOfBlock);
            else
                data = null;
        }
    }
}