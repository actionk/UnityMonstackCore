using System.Collections.Generic;
using UnityEngine;

namespace Plugins.UnityMonstackCore.Extensions
{
    public static class GameObjectExtension
    {
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
    }
}