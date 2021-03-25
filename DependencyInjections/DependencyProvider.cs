#region import

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using Plugins.UnityMonstackCore.Loggers;
using Unity.Entities;
using UnityEngine;
using Object = UnityEngine.Object;

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

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        public static void Reset()
        {
            INJECTABLE_TYPES.Clear();
            INITIALIZE_ON_STARTUP.Clear();
            DEPENDENCIES.Clear();
            INSTANTIABLE_TYPES.Clear();
            CURRENTLY_LOADING_DEPENDENCIES.Clear();
            IS_INITIALIZED = false;
        }

        public static void Initialize(Assembly projectAssembly)
        {
            UnityLogger.Log($"Loading dependencies from assembly {projectAssembly.GetName().Name}");

            InitializeForAssembly(projectAssembly);

            IS_INITIALIZED = true;

            foreach (var typeToInitializeOnStartup in INITIALIZE_ON_STARTUP)
                LoadIfUnresolved(typeToInitializeOnStartup.Type);
            INITIALIZE_ON_STARTUP.Clear();
        }

        public static void Shutdown(Assembly assembly)
        {
            var injectableTypes = INJECTABLE_TYPES.ToArray();
            foreach (var type in injectableTypes)
            {
                if (type.Assembly == assembly)
                    INJECTABLE_TYPES.Remove(type);
            }

            var instantiableTypes = INSTANTIABLE_TYPES.Keys.ToArray();
            foreach (var type in instantiableTypes)
            {
                if (type.Assembly == assembly)
                    INSTANTIABLE_TYPES.Remove(type);
            }

            var clearTypes = new List<Type>();
            foreach (var keyValuePair in DEPENDENCIES)
            {
                if (keyValuePair.Key.Assembly == assembly)
                {
                    clearTypes.Add(keyValuePair.Key);
                    continue;
                }

                var clearObjects = new List<object>();
                foreach (var value in keyValuePair.Value)
                {
                    if (value.GetType().Assembly == assembly)
                        clearObjects.Add(value);
                }

                foreach (var objectToClear in clearObjects)
                    keyValuePair.Value.Remove(objectToClear);
            }

            foreach (var type in clearTypes)
            {
                var hashSet = DEPENDENCIES[type];
                hashSet.Clear();
                DEPENDENCIES.Remove(type);
            }
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
#if UNITY_EDITOR
            if (typeof(ComponentSystem).IsAssignableFrom(type))
            {
                UnityLogger.Error($"Seems like somebody has to go to sleep if you add [Inject] to ComponentSystem: {type} :facepalm:");
                return;
            }
#endif

            INJECTABLE_TYPES.Add(type);

            if (injectAttribute.CreateOnStartup)
                INITIALIZE_ON_STARTUP.Add(new InitializeOnStartup {MethodInfo = resolveMethodInfo, Type = type});
        }

        public static T Add<T>(T objectToSet)
        {
            Add(objectToSet.GetType(), objectToSet);
            return objectToSet;
        }

        public static T Add<T>(Type type, T objectToSet)
        {
            if (!DEPENDENCIES.ContainsKey(type)) DEPENDENCIES[type] = new HashSet<object>();
            DEPENDENCIES[type].Add(objectToSet);
            return objectToSet;
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
                throw new InvalidOperationException($"Failed to resolve type {type} as it's not injected");

            if (DEPENDENCIES[type].Count > 1)
                throw new InvalidOperationException($"Tried to resolve a single object whereas there are a few implementing type {type}: {DEPENDENCIES[type]}");

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
            if (DEPENDENCIES.ContainsKey(type))
                return;

            if (CURRENTLY_LOADING_DEPENDENCIES.Contains(type))
                throw new AccessViolationException(
                    $"An attempt to load class that is being loaded. Please check that you don't have any dependency usage of class {type}" + " in its constructor");

            if (type.IsSubclassOf(typeof(MonoBehaviour)) && type.GetCustomAttribute<InjectAttribute>() != null)
            {
                var instance = Object.FindObjectOfType(type);
                if (instance != null)
                    Add(type, instance);
                else
                    UnityLogger.Error($"Failed to instantiate MonoBehaviour object of type {type}: no object found in the scene");

                return;
            }

            if (Application.isEditor && !Application.isPlaying)
            {
                if (!DEPENDENCIES.ContainsKey(type))
                {
                    UnityLogger.Debug($"Initializing dependency {type} inside the editor");
                    InstantiateObject(type);
                }

                return;
            }

            if (!IS_INITIALIZED)
                throw new AccessViolationException(
                    "Tried to resolve without Initializing " + typeof(DependencyProvider));

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
                    if (argument.IsOptional)
                    {
                        args.Add(Type.Missing);
                        continue;
                    }

                    if (argument.ParameterType.IsInstanceOfType(typeof(List<>)))
                        throw new InvalidOperationException("Injecting lists into constructor isn't supported");

                    var resolveByType = ResolveByType(argument.ParameterType);
                    args.Add(resolveByType);
                }

                newObject = Activator.CreateInstance(type,
                    BindingFlags.CreateInstance | BindingFlags.Public | BindingFlags.Instance | BindingFlags.OptionalParamBinding,
                    null,
                    args.ToArray(),
                    CultureInfo.CurrentCulture);
            }
            else
            {
                newObject = Activator.CreateInstance(type);
            }

            foreach (var method in newObject.GetType().GetMethods())
            {
                var afterInstantiationAttribute = method.GetCustomAttribute<OnCreate>();
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
                var autowiredAttribute = Attribute.GetCustomAttribute(constructor, typeof(DefaultConstructorAttribute));
                if (autowiredAttribute != null) return constructor;
            }

            if (constructorInfos.Length > 1)
            {
                var defaultConstructor = constructorInfos.FirstOrDefault(x => x.GetParameters().Length == 0);
                if (defaultConstructor == null)
                    throw new Exception($"Class {type} has more than 1 default constructor. Please define one of those as [{nameof(DefaultConstructorAttribute)}]");

                return defaultConstructor;
            }

            if (constructorInfos.Length == 1) return constructorInfos[0];

            return null;
        }

        public static bool IsInitialized()
        {
            return IS_INITIALIZED;
        }

        private class
            InitializeOnStartup
        {
            public MethodInfo MethodInfo;
            public Type Type;
        }
    }
}