using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Plugins.UnityMonstackCore.Extensions
{
    public static class GameObjectExtension
    {
        public static T GetOrAddComponent<T>(this GameObject gameObject) where T : MonoBehaviour
        {
            var existingComponent = gameObject.GetComponent<T>();
            if (existingComponent == null)
                return gameObject.AddComponent<T>();
                
            return existingComponent;
        }
        
        public static void DestroyGameObject(this Transform transform, bool immediate = false)
        {
            if (immediate)
                Object.DestroyImmediate(transform.gameObject);
            else
                Object.Destroy(transform.gameObject);
        }

        public static void DestroyChildren(this Transform transform, bool immediate = false)
        {
            var children = new List<GameObject>();
            foreach (Transform childTransform in transform)
                children.Add(childTransform.gameObject);
            foreach (var gameObject in children)
            {
                if (immediate)
                    Object.DestroyImmediate(gameObject);
                else
                    Object.Destroy(gameObject);
            }
        }

        public static void DestroyChildren(this Transform transform, Predicate<GameObject> predicate, bool immediate = false)
        {
            var children = new List<GameObject>();
            foreach (Transform childTransform in transform)
                children.Add(childTransform.gameObject);
            foreach (var gameObject in children)
            {
                if (!predicate.Invoke(gameObject))
                    continue;

                if (immediate)
                    Object.DestroyImmediate(gameObject);
                else
                    Object.Destroy(gameObject);
            }
        }
    }
}