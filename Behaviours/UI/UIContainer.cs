using System;
using System.Collections.Generic;
using Plugins.UnityMonstackCore.Extensions;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Plugins.Shared.UnityMonstackCore.Behaviours.UI
{
    public class UIContainer : MonoBehaviour
    {
        [AssetsOnly]
        public GameObject prefab;

        [NonSerialized, ShowInInspector]
        public readonly List<GameObject> elements = new List<GameObject>();

        public GameObject CreateElement()
        {
            var element = Instantiate(prefab, transform);
            elements.Add(element);
            return element;
        }

        public T CreateElement<T>() where T: MonoBehaviour
        {
            var element = Instantiate(prefab, transform);
            elements.Add(element);
            return element.GetComponent<T>();
        }

        public void Clear()
        {
            foreach (var element in elements)
            {
                element.transform.DestroyGameObject();
            }
            elements.Clear();
        }
    }
}