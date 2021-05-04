#region import

using System;
using System.Collections.Generic;
using System.Diagnostics;
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
        private static HashSet<Assembly> InitializedAssemblies = new HashSet<Assembly>();
        private static HashSet<Type> InjectableTypes = new HashSet<Type>();
        private static List<InitializeOnStartup> InstantiateOnStartup = new List<InitializeOnStartup>();
        private static Dictionary<Type, HashSet<object>> Instances = new Dictionary<Type, HashSet<object>>();
        private static HashSet<Type> CurrentlyLoadingInstances = new HashSet<Type>();

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        public static void Reset()
        {
            InitializedAssemblies.Clear();
            InjectableTypes.Clear();
            InstantiateOnStartup.Clear();
            Instances.Clear();
            CurrentlyLoadingInstances.Clear();
        }

        public static void Initialize(Assembly assembly)
        {
            if (InitializedAssemblies.Contains(assembly))
                return;

            var stopwatch = new Stopwatch();
            stopwatch.Start();
            InitializedAssemblies.Add(assembly);

            InitializeForAssembly(assembly);

            foreach (var typeToInitializeOnStartup in InstantiateOnStartup)
                LoadIfUnresolved(typeToInitializeOnStartup.Type);
            InstantiateOnStartup.Clear();

            stopwatch.Stop();

            UnityLogger.Log($"Loading dependencies from assembly [{assembly.GetName().Name}] done in [{stopwatch.ElapsedMilliseconds} ms]");
        }

        public static void Shutdown(Assembly assembly)
        {
            var injectableTypes = InjectableTypes.ToArray();
            foreach (var type in injectableTypes)
            {
                if (type.Assembly == assembly)
                    InjectableTypes.Remove(type);
            }

            var clearTypes = new List<Type>();
            foreach (var keyValuePair in Instances)
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
                var hashSet = Instances[type];
                hashSet.Clear();
                Instances.Remove(type);
            }
        }

        private static void InitializeForAssembly(Assembly assembly)
        {
            var resolveMethodInfo = typeof(DependencyProvider).GetMethod("Resolve");
            foreach (var type in assembly.GetTypes())
            {
                var injectAttribute = (InjectAttribute) Attribute.GetCustomAttribute(type, typeof(InjectAttribute));
                if (injectAttribute != null) AddInjectableType(type, injectAttribute, resolveMethodInfo);
            }
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
            InjectableTypes.Add(type);

            if (injectAttribute.CreateOnStartup)
                InstantiateOnStartup.Add(new InitializeOnStartup {MethodInfo = resolveMethodInfo, Type = type});

            // if there are already interfaces initialized - instantiate those classes immediatelly
            var instances = Instances.Keys.ToArray();
            foreach (var instanceType in instances)
            {
                if (instanceType.IsAssignableFrom(type))
                {
                    LoadIfUnresolved(type);
                    foreach (var injectable in Instances[type]) Add(instanceType, injectable);
                }
            }
        }

        public static T Add<T>(T objectToSet)
        {
            Add(objectToSet.GetType(), objectToSet);
            return objectToSet;
        }

        public static T Add<T>(Type type, T objectToSet)
        {
            if (!Instances.ContainsKey(type)) Instances[type] = new HashSet<object>();
            Instances[type].Add(objectToSet);
            return objectToSet;
        }

        public static bool IsResolvable<T>() where T : class
        {
            return Instances.ContainsKey(typeof(T)) || InjectableTypes.Contains(typeof(T));
        }

        public static T Resolve<T>() where T : class
        {
            var type = typeof(T);
            return (T) ResolveByType(type);
        }

        public static T ResolveIfPossible<T>() where T : class
        {
            try
            {
                var type = typeof(T);
                return (T) ResolveByType(type);
            }
            catch (Exception e)
            {
                return null;
            }
        }

        public static T ResolveByType<T>(Type type) where T : class
        {
            return (T) ResolveByType(type);
        }

        public static object ResolveByType(Type type)
        {
            LoadIfUnresolved(type);
            if (!Instances.ContainsKey(type))
                throw new InvalidOperationException($"Failed to resolve type {type} as it's not injected");

            if (Instances[type].Count > 1)
                throw new InvalidOperationException($"Tried to resolve a single object whereas there are a few implementing type {type}: {Instances[type]}");

            var enumerator = Instances[type].GetEnumerator();
            enumerator.MoveNext();
            return enumerator.Current;
        }

        public static IEnumerable<T> ResolveList<T>() where T : class
        {
            var type = typeof(T);
            LoadIfUnresolved(type);
            if (!Instances.ContainsKey(type)) return Enumerable.Empty<T>();
            return Instances[type].Cast<T>().ToList();
        }

        private static void LoadIfUnresolved(Type type)
        {
            if (Instances.ContainsKey(type))
                return;

            if (CurrentlyLoadingInstances.Contains(type))
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
                if (!Instances.ContainsKey(type))
                {
                    UnityLogger.Debug($"Initializing dependency {type} inside the editor");
                    InstantiateObject(type);
                }

                return;
            }

            if (!InitializedAssemblies.Contains(type.Assembly))
                throw new AccessViolationException(
                    "Tried to resolve without Initializing " + typeof(DependencyProvider));

            CurrentlyLoadingInstances.Add(type);

            foreach (var injectableType in InjectableTypes)
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
                    if (!Instances.ContainsKey(injectableType)) LoadIfUnresolved(injectableType);

                    foreach (var injectable in Instances[injectableType]) Add(type, injectable);
                }

            CurrentlyLoadingInstances.Remove(type);
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

        private class InitializeOnStartup
        {
            public MethodInfo MethodInfo;
            public Type Type;
        }
    }
}