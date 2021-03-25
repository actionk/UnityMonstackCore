using System;
using System.Collections.Generic;

namespace Plugins.Shared.UnityMonstackCore.Utils
{
    public static class ReflectionUtils
    {
        public static Type[] GetAllDerivedTypes(this AppDomain aAppDomain, Type aType)
        {
            var result = new List<Type>();
            var assemblies = aAppDomain.GetAssemblies();
            foreach (var assembly in assemblies)
            {
                var types = assembly.GetTypes();
                foreach (var type in types)
                {
                    if (type.IsSubclassOf(aType))
                        result.Add(type);
                }
            }

            return result.ToArray();
        }

        public static Type[] GetAllDerivedTypes<T>(this AppDomain aAppDomain)
        {
            return GetAllDerivedTypes(aAppDomain, typeof(T));
        }

        public static Type[] GetAllDerivedTypes<T>()
        {
            return GetAllDerivedTypes(AppDomain.CurrentDomain, typeof(T));
        }

        public static Type[] GetTypesWithInterface(this AppDomain aAppDomain, Type aInterfaceType, bool excludeAbstract = false)
        {
            var result = new List<Type>();
            var assemblies = aAppDomain.GetAssemblies();
            foreach (var assembly in assemblies)
            {
                var types = assembly.GetTypes();
                foreach (var type in types)
                {
                    if (excludeAbstract && type.IsAbstract)
                        continue;

                    if (aInterfaceType.IsAssignableFrom(type))
                        result.Add(type);
                }
            }

            return result.ToArray();
        }

        public static Type[] GetTypesWithInterface<T>(this AppDomain aAppDomain)
        {
            return GetTypesWithInterface(aAppDomain, typeof(T));
        }

        public static Type[] GetAllTypesWithInterface<T>(bool excludeAbstract = false)
        {
            return GetTypesWithInterface(AppDomain.CurrentDomain, typeof(T), excludeAbstract);
        }
    }
}