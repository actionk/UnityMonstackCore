#region import

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Plugins.UnityMonstackCore.Logs;
using Sirenix.Utilities;
using UnityEngine;

#endregion

namespace Plugins.UnityMonstackCore.DependencyInjections
{
    public static class DependencyProvider
    {
        private static bool IS_INITIALIZED;
        private static List<Type> INJECTABLE_TYPES = new List<Type>();
        private static List<InitializeOnStartup> INITIALIZE_ON_STARTUP = new List<InitializeOnStartup>();
        private static Dictionary<Type, HashSet<object>> DEPENDENCIES = new Dictionary<Type, HashSet<object>>();
        private static Dictionary<Type, HashSet<Type>> INSTANTIABLE_TYPES = new Dictionary<Type, HashSet<Type>>();
        private static HashSet<Type> CURRENTLY_LOADING_DEPENDENCIES = new HashSet<Type>();

#if UNITY_EDITOR
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        public static void Reset()
        {
            INJECTABLE_TYPES = new List<Type>();
            INITIALIZE_ON_STARTUP = new List<InitializeOnStartup>();
            DEPENDENCIES = new Dictionary<Type, HashSet<object>>();
            INSTANTIABLE_TYPES = new Dictionary<Type, HashSet<Type>>();
            CURRENTLY_LOADING_DEPENDENCIES = new HashSet<Type>();
            IS_INITIALIZED = false;
        }
#endif

        public static void Initialize(Assembly projectAssembly)
        {
            InitializeForAssembly(projectAssembly);

            IS_INITIALIZED = true;

            foreach (var typeToInitializeOnStartup in INITIALIZE_ON_STARTUP)
                LoadIfUnresolved(typeToInitializeOnStartup.Type);
        }

        private static void InitializeForAssembly(Assembly assembly)
        {
            var resolveMethodInfo = typeof(DependencyProvider).GetMethod("Resolve");
            foreach (var type in assembly.GetTypes())
            {
                var instantiableAttribute =
                    (InstantiableAttribute) Attribute.GetCustomAttribute(type, typeof(InstantiableAttribute));
                if (instantiableAttribute != null) AddInstantiableType(type, instantiableAttribute, resolveMethodInfo);

                var injectAttribute = (InjectAttribute) Attribute.GetCustomAttribute(type, typeof(InjectAttribute));
                if (injectAttribute != null) AddInjectableType(type, injectAttribute, resolveMethodInfo);
            }
        }

        public static void InitializeForTests()
        {
            IS_INITIALIZED = true;
            DEPENDENCIES.Clear();
        }

        private static void AddInstantiableType(Type type, InstantiableAttribute instantiableAttribute,
            MethodInfo resolveMethodInfo)
        {
            if (!INSTANTIABLE_TYPES.ContainsKey(instantiableAttribute.BasicType))
                INSTANTIABLE_TYPES[instantiableAttribute.BasicType] = new HashSet<Type>();
            INSTANTIABLE_TYPES[type].Add(type);
        }

        private static void AddInjectableType(Type type, InjectAttribute injectAttribute, MethodInfo resolveMethodInfo)
        {
            INJECTABLE_TYPES.Add(type);

            if (injectAttribute.CreateOnStartup)
                INITIALIZE_ON_STARTUP.Add(new InitializeOnStartup {MethodInfo = resolveMethodInfo, Type = type});
        }

        public static void Add<T>(T objectToSet)
        {
            Add(objectToSet.GetType(), objectToSet);
        }

        public static void Add<T>(Type type, T objectToSet)
        {
            if (!DEPENDENCIES.ContainsKey(type)) DEPENDENCIES[type] = new HashSet<object>();
            DEPENDENCIES[type].Add(objectToSet);
        }

        public static T Resolve<T>() where T : class
        {
            var type = typeof(T);
            return (T) ResolveByType(type);
        }

        public static T ResolveByType<T>(Type type) where T : class
        {
            return (T) ResolveByType(type);
        }

        public static object ResolveByType(Type type)
        {
            LoadIfUnresolved(type);
            if (!DEPENDENCIES.ContainsKey(type))
                throw new InvalidOperationException(string.Format("Failed to resolve type {0} as it's not injected",
                    type));
            if (DEPENDENCIES[type].Count > 1)
                throw new InvalidOperationException(
                    string.Format("Tried to resolve a single object whereas there are a few implementing type {0}: {1}",
                        type, DEPENDENCIES[type]));

            var enumerator = DEPENDENCIES[type].GetEnumerator();
            enumerator.MoveNext();
            return enumerator.Current;
        }

        public static IEnumerable<T> ResolveList<T>() where T : class
        {
            var type = typeof(T);
            LoadIfUnresolved(type);
            if (!DEPENDENCIES.ContainsKey(type)) return Enumerable.Empty<T>();
            return DEPENDENCIES[type].Cast<T>().ToList();
        }

        public static IEnumerable<Type> GetInstantiableTypesByBasicType(Type basicType)
        {
            if (!INSTANTIABLE_TYPES.ContainsKey(basicType)) return Enumerable.Empty<Type>();
            return INSTANTIABLE_TYPES[basicType];
        }

        private static void LoadIfUnresolved(Type type)
        {
            if (!IS_INITIALIZED)
                throw new AccessViolationException(
                    "Tried to resolve without Initializing " + typeof(DependencyProvider));
            if (CURRENTLY_LOADING_DEPENDENCIES.Contains(type))
                throw new AccessViolationException(
                    string.Format(
                        "An attempt to load class that is being loaded. Please check that you don't have any dependency usage of class {0}" +
                        " in its constructor", type));
            if (!DEPENDENCIES.ContainsKey(type))
            {
                CURRENTLY_LOADING_DEPENDENCIES.Add(type);

                foreach (var injectableType in INJECTABLE_TYPES)
                    if (injectableType == type)
                    {
                        try
                        {
                            InstantiateObject(type);
                        }
                        catch (Exception ex)
                        {
                            UnityLogger.Error("Tried to instantiate class [" + type + "], but failed because of: " + ex);
                        }
                    }
                    else if (type.IsAssignableFrom(injectableType))
                    {
                        if (!DEPENDENCIES.ContainsKey(injectableType)) LoadIfUnresolved(injectableType);

                        foreach (var injectable in DEPENDENCIES[injectableType]) Add(type, injectable);
                    }

                CURRENTLY_LOADING_DEPENDENCIES.Remove(type);
            }
        }

        private static void InstantiateObject(Type type)
        {
            object newObject;

            var autowiredConstructor = GetAutowiredOrDefaultConstructor(type);
            if (autowiredConstructor != null)
            {
                var parameterInfos = autowiredConstructor.GetParameters();
                var args = new List<object>();
                foreach (var argument in parameterInfos)
                {
                    if (argument.ParameterType.IsInstanceOfType(typeof(List<>)))
                        throw new InvalidOperationException("Injecting lists into constructor isn't supported");

                    var resolveByType = ResolveByType(argument.ParameterType);
                    args.Add(resolveByType);
                }

                newObject = Activator.CreateInstance(type, args.ToArray());
            }
            else
            {
                newObject = Activator.CreateInstance(type);
            }

            foreach (var method in newObject.GetType().GetMethods())
            {
                var afterInstantiationAttribute = method.GetAttribute<OnCreate>();
                if (afterInstantiationAttribute != null)
                {
                    if (method.GetParameters().Length > 0)
                        throw new Exception(
                            $"{newObject.GetType().Name}::{method.Name} has arguments. [OnCreate] only works on empty methods");

                    method.Invoke(newObject, new object[] { });
                }
            }

            Add(type, newObject);
        }

        private static ConstructorInfo GetAutowiredOrDefaultConstructor(Type type)
        {
            var constructorInfos = type.GetConstructors();
            foreach (var constructor in constructorInfos)
            {
                var autowiredAttribute = Attribute.GetCustomAttribute(constructor, typeof(AutowiredAttribute));
                if (autowiredAttribute != null) return constructor;
            }

            if (constructorInfos.Length > 1)
                throw new Exception("Class " + type +
                                    " has more than 1 default constructor. Please define one of those as [Autowired]");

            if (constructorInfos.Length == 1) return constructorInfos[0];

            return null;
        }

        public static bool IsInitialized()
        {
            return IS_INITIALIZED;
        }

        private class InitializeOnStartup
        {
            public MethodInfo MethodInfo;
            public Type Type;
        }
    }
}