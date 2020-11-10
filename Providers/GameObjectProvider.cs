using System;
using UnityEngine;

namespace Plugins.UnityMonstackCore.Providers
{
    public class GameObjectProvider
    {
        public static T FindGameObject<T>(string name) where T : Component
        {
            return GameObject.Find(name).GetComponent<T>()
                   ?? throw new Exception($"GameObject with name {name} not found");
        }

        public static T FindGameObjectByName<T>() where T : Component
        {
            var name = typeof(T).Name;
            return GameObject.Find(name).GetComponent<T>()
                   ?? throw new Exception($"GameObject with name {name} not found");
        }
    }
}