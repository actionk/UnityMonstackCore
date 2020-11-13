using Plugins.UnityMonstackCore.Loggers;
using UnityEngine;

namespace Plugins.UnityMonstackCore.Providers
{
    public class ResourceProvider
    {
        private static T CheckForNullAndReturn<T>(T checkingObject, string errorMessage)
        {
            if (checkingObject == null) UnityLogger.Error(errorMessage);

            return checkingObject;
        }

        public static GameObject GetPrefab(string key)
        {
            var gameObject = Resources.Load<GameObject>("Prefabs/" + key);
            return CheckForNullAndReturn(gameObject, "Gameobject wasn't loaded, path: " + key);
        }

        public static Material GetMaterial(string key)
        {
            var material = Resources.Load<Material>("Materials/" + key);
            return CheckForNullAndReturn(material, "Material wasn't loaded, path: " + key);
        }

        public static string GetJSON(string key)
        {
            var text = Resources.Load<TextAsset>(key).text;
            return CheckForNullAndReturn(text, "Text wasn't loaded, path: " + key);
        }

        public static Sprite GetSprite(string key)
        {
            var sprite = Resources.Load<Sprite>("Sprites/" + key);
            return CheckForNullAndReturn(sprite, "Sprite wasn't loaded, path: " + key);
        }

        public static Sprite TryGetSprite(string key)
        {
            return Resources.Load<Sprite>("Sprites/" + key);
        }

        public static T GetSettings<T>(string settingsName) where T : Object
        {
            return Resources.Load<T>("Settings/" + settingsName);
        }

        public static GameObject TryGetPrefab(string key)
        {
            return Resources.Load<GameObject>("Prefabs/" + key);
        }
    }
}