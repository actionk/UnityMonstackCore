# Monstack Core

Core library of Monstack Tools. Contains extras that are used in different Monstack Tools projects, such as:
* [Content Loader](https://github.com/actionk/UnityMonstackContentLoader)

Contents:

* [Install]
* [Dependendecy Injection](#dependency-injection)
* [Extensions](#extensions)
* [Logger](#logger)
* [Providers](#providers)
* * [Resource Provider](#resources-provider)
* [Services](#services)
* * [Event Service](#event-service)
* [Utils](#utils)

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
