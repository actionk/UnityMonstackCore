using System;
using System.Collections.Generic;
using Plugins.UnityMonstackCore.Loggers;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using Sirenix.Utilities;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;

#endif

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
        [InlineButton(nameof(OpenSource), "?")]
        public string type = "";

        [NonSerialized, OdinSerialize, ShowInInspector, InlineProperty, HideReferenceObjectPicker, HideLabel, HideIf(nameof(IsEmpty))]
        public T data;

        public bool IsEmpty => data == null;
        protected virtual IEnumerable<ValueDropdownItem> GetTypes => FunctionBlocksManager.Instance.GetValueDropdownByType<T>();

        private void OpenSource()
        {
#if UNITY_EDITOR
            if (data == null)
            {
                UnityLogger.Log("No type defined yet");
                return;
            }

            var typeOfBlock = FunctionBlocksManager.Instance.GetBlockTypesByType<T>()[type];
            var asset = AssetDatabase.LoadAssetAtPath("Assets/" + typeOfBlock.Namespace!.Replace(".", "/") + "/" + typeOfBlock.Name + ".cs", typeof(MonoScript));
            AssetDatabase.OpenAsset(asset);
#endif
        }

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