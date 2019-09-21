# microsoft-dependencyinjection-decorator
An extension library to enable usage of the Decorator Pattern whilst using ASP.NET Core Dependency Injection

## Background
The ASP.NET Core [Dependency Injection library][https://github.com/aspnet/Extensions/tree/master/src/DependencyInjection] provides a built-in Inversion of Control container capability. The opinions and community of this IoC container implementation have taken the stance of keeping the feature set minimal and defined,
for the good reason of maintaining a simple API that supplements the framework for immediately usability and caters the majority of use cases. It is recommended to use other IoC container libraries for more advanced use cases

One common pattern that has emerged in the support of seperation of concerns is also the Decorator pattern. This is one case that the ASP.NET Core library does not directly support, as the container only allows the registration of 1 implementation per type. This library attempts to hide away the complexity of registrating decorators, and add an extension to the existing API in order to enable even more use cases to be covered.


## Usage

Decorators may be registered for an existing implementation by invocating the _Decorate_ extension method for a ServiceCollection:

```csharp
var services = new ServiceCollection();

serviceCollection
  .AddTransient<IMyInterface, MyImplementation>()
  .Decorate<IMyInterface, MyDecorator>();
  
var provider = serviceCollection.BuildServiceProvider();
```
