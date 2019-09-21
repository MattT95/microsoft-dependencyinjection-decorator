using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ServiceCollectionDecoratorExtensions
    {
        public delegate object CreateObject(object[] args);
        
        public static CreateObject ConstructCreateObjectDelegate(ConstructorInfo constructor)
        {
            var parameters = constructor.GetParameters();

            var paramExpr = Expression.Parameter(typeof(object[]), "args");
            
            var args = new Expression[parameters.Length];

            for (var i = 0; i < parameters.Length; i++)
            {
                var paramType = parameters[i].ParameterType;
                
                var indexExpr = Expression.Constant(i);

                var arrayExpr = Expression.ArrayIndex(paramExpr, indexExpr);

                var castExpr = Expression.Convert(arrayExpr, paramType);
                
                args[i] = castExpr;
            }
            
            var newExp = Expression.New(constructor, args);
            
            var lambda = Expression.Lambda(typeof(CreateObject), newExp, paramExpr);

            return (CreateObject) lambda.Compile();
        }
        
        public static IServiceCollection Decorate<TService, TImplementation>(this IServiceCollection serviceCollection) where TImplementation : TService
        {
            var targetServiceDescriptor = GetServiceDescriptorForType<TService>(serviceCollection);

            var constructor = typeof(TImplementation).GetConstructors()[0];
            var parameters = constructor.GetParameters();

            var decoratorDelegate = ConstructCreateObjectDelegate(constructor);
            var innerService = CreateServiceFromDescriptor(targetServiceDescriptor);

            var decoratorFactory = new Func<IServiceProvider, object>(services =>
            {

                var args = new object[parameters.Length];
                
                for (var i = 0; i < parameters.Length; i++)
                {
                    var param = parameters[i];
                    
                    
                    if (param.ParameterType == typeof(TService))
                    {
                        args[i] = innerService.Invoke(services);
                        continue;
                    }
    
                    var arg = services.GetRequiredService(param.ParameterType);
                    args[i] = arg;
                }

                return (TService) decoratorDelegate.Invoke(args);
            });
            
            var decoratedServiceDescriptor = new ServiceDescriptor(typeof(TService), decoratorFactory, targetServiceDescriptor.Lifetime);

            serviceCollection.Remove(targetServiceDescriptor);
            serviceCollection.Add(decoratedServiceDescriptor);

            return serviceCollection;
        }

        private static Func<IServiceProvider, object> CreateServiceFromDescriptor(ServiceDescriptor serviceDescriptor)
        {
            if (serviceDescriptor.ImplementationInstance != null)
            {
                return (s) => serviceDescriptor.ImplementationInstance;
            }

            if (serviceDescriptor.ImplementationFactory != null)
            {
                return (s) => serviceDescriptor.ImplementationFactory(s);
            }

            var serviceConstructor = serviceDescriptor.ImplementationType.GetConstructors()[0];
            var createServiceDelegate = ConstructCreateObjectDelegate(serviceConstructor);

            
            return (s) =>
            {
                var args = ResolveConstructorParams(serviceConstructor, s);
                return createServiceDelegate.Invoke(args);
            };
        }

        private static object CreateServiceByReflection(IServiceProvider serviceProvider, Type implementationType)
        {
            var constructor = implementationType.GetConstructors()[0];

            var parameters = ResolveConstructorParams(constructor, serviceProvider);

            return constructor.Invoke(parameters);
        }

        private static object[] ResolveConstructorParams(MethodBase constructor, IServiceProvider serviceProvider)
        {
            var parameters = constructor.GetParameters().ToArray();
            var parametersValues = new object[parameters.Length];

            for (var i = 0; i < parameters.Length; i++)
            {
                var parameter = parameters[i];

                parametersValues[i] = serviceProvider.GetRequiredService(parameter.ParameterType);
            }

            return parametersValues;
        }

        private static ServiceDescriptor GetServiceDescriptorForType<TService>(IServiceCollection serviceCollection)
        {
            foreach (var service in serviceCollection)
            {
                if (service.ServiceType == typeof(TService))
                {
                    return service;
                }
            }

            throw new Exception($"No service found to be in collection for type: {typeof(TService)}");
        }
    }
}