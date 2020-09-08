# Monstack Core

Core library of Monstack Tools. Contains extras that are used in different Monstack Tools projects, such as:
* [Content Loader](https://github.com/actionk/UnityMonstackContentLoader)

# Table of Contents

* [Install](#install)
* [Dependendecy Injection](#dependency-injection)
* [Extensions](#extensions)
* [Logger](#logger)
* [Providers](#providers)
* * [Resource Provider](#resources-provider)
* [FPS Counter](#fps-counter)
* [Services](#services)
* * [Event Service](#event-service)

# Install

You can either just put the files into `Assets/Plugins/UnityMonstackCore` or add it as a submodule:
```sh
git submodule add https://github.com/actionk/UnityMonstackCore.git Assets/Plugins/UnityMonstackCore
```

# Dependency Injection

## Initializing dependency injection

Execute this when starting your game:

```cs
DependencyProvider.Initialize(Assembly.GetExecutingAssembly());
```

If you have injectable classes in another assembly, you should also initialize them manually. For example, if your injectable classes are in the same assembly as the UnityMonstackCore (Plugins assembly?), simply do this:

```cs
DependencyProvider.Initialize(typeof(DependencyProvider).Assembly);
```

## Mapping class as injectable

To make a class injectable you simply add an annotation `[Inject]` to it:

```cs
[Inject]
public class MyInjectableClass
{
}
```

## Obtaining an instance of injectable class

```cs
DependencyProvider.Resolve<MyInjectableClass>();
```

## Resolving the interface

The dependency injection system will automatically scan the hierarchy of your class and provide a dependency link for all the interfaces in a tree.

```cs
[Inject]
public class MyInjectableClass : MyInterface
{
}
```

Can be injected as:
```cs
DependencyProvider.Resolve<MyInterface>();
```

## Resolving a list of instances

When you want to provide of list of interface implementations, you simply inherit from the same interfaces and resolve the list this way:

```cs
DependencyProvider.ResolveList<MyInterface>();
```
## Constructor injection

When creating an injectable class, simply add a default constructor with its dependencies and they will be injected automatically.

```cs
[Inject]
public class MyInjectableClass : MyInterface
{
  public MyInjectableClass(AnotherInjectableClass dependency) { }
}
```

If you want to specify the exact constructor that should be used, mark it with `[Autowired]` annotation:

```cs
[Inject]
public class MyInjectableClass : MyInterface
{
  public MyInjectableClass(int testValue) { }
  
  [Autowired]
  public MyInjectableClass(AnotherInjectableClass dependency) { }
}
```

## Instantiating a class immediately on startup 

By default all classes are being instantied once they are resolved. To do this immediately on DI initialization, add `CreateOnStartup=true` to [Inject] attribute:

```cs
[Inject(CreateOnStartup = true)]
```

# Extensions

The library contains some useful extensions that you might need in your everyday routine. Some examples:

```cs
var pointX = x.Remap(0, size.x, 0, maskSize.x);
```

```cs
gameObject.transform.DestroyChildren();
```

```cs
Camera.main.GetOrthographicBounds();
```

```cs
dictionary.ComputeIfAbsent(key, x => new Value {id = x});
```

# Loggers

The library contains a simple logger as a wrapper to original Unity's logger.

```cs
UnityLogger.Log(message)
UnityLogger.Debug(message)
UnityLogger.Info(message)
UnityLogger.Warning(message)
UnityLogger.Warning(message)
UnityLogger.Error(message)
UnityLogger.Format(message)
```

### Setting logger level

```cs
UnityLogger.SetLevel(LoggingLevel.ERROR)
```

There are four logging levels:

```cs
public enum LoggingLevel
        {
            ERROR,
            WARNING,
            INFO,
            DEBUG
        }
```

# Providers

## Resource Provider

ResourceProvider is a simple util that resolves resources from `Resources` folder:

```cs
ResourceProvider.GetPrefab(prefabName); // Looks for Resources/Prefabs/<prefabName>
ResourceProvider.GetMaterial(materialName); // Looks for Resources/Prefabs/<materialName>
ResourceProvider.GetJSON(jsonName); // Looks for Resources/Prefabs/<jsonName>
ResourceProvider.GetSprite(spriteName); // Looks for Resources/Prefabs/<spriteName>
```

# FPS Counter

If you need a FPS counter in your project, just use the Prefab "Prefabs/FPSCounter"

# Services

## Event Service

If you want to create a simple EventSystem, first of all you create a enum for your event types:

```cs
public enum EventType {
  MY_EVENT_1,
  MY_EVENT_2
}
```

Then you create an `EventService` by inheriting from `AbstractEventService`:

```cs
[Inject]
public class EventService : AbstractEventService<EventType> {

}
```

### Subscribing to events

```cs
public void Start() {
  DependencyProvider.Resolve<EventManager>().CreateListener<ShowStorageUIEvent>(EventType.MY_EVENT_1, ShowStorageEvent);
}

private void ShowStorageEvent(ShowStorageUIEvent eventData)
{
  // on event
}
```

### Unsubscribing all by event type

```cs
DependencyProvider.Resolve<EventManager>().RemoveSubscribersForEvent(EventType.MY_EVENT_1);
```

### Unsubscribing by target

```cs
DependencyProvider.Resolve<EventManager>().RemoveSubscribersForTarget(this)
```

### Dispatching an event

With event data:

```cs
DependencyProvider.Resolve<EventManager>()..DispatchEvent(UIEvent.MY_EVENT_1, new ShowStorageUIEvent
            {
                objectId = objectId,
            });
```
No event data:

```cs
DependencyProvider.Resolve<EventManager>()..DispatchEvent(UIEvent.MY_EVENT_2);
```
