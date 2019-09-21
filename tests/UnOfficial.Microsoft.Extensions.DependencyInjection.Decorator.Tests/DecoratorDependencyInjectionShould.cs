using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace UnOfficial.Microsoft.Extensions.DependencyInjection.Decorator.Tests
{
    public class DecoratorDependencyInjectionShould
    {
        [Test]
        public void ResolveVariablesOfImplementation()
        {
            var serviceCollection = new ServiceCollection();

            serviceCollection.AddSingleton<ITestDependency, TestDependency>();
            serviceCollection.AddSingleton<ITestInterface, TestImplementation>();
            serviceCollection.Decorate<ITestInterface, DecoratingTestImplementationWithDependencies>();
            
            var serviceProvider = serviceCollection.BuildServiceProvider();

            var actual = serviceProvider.GetService<ITestInterface>();

            actual
                .Should().BeOfType<DecoratingTestImplementationWithDependencies>()
                .Which
                .TestDependency.Should().BeOfType<TestDependency>();
        }
        
        
        // TODO :: Add more tests to appropriately cover different lifestyles being registered
        [Test]
        public void RespectLifestyleOfRegistrationBeingDecorated()
        {
            var implementationInstance = new TestImplementation();

            var serviceCollection = new ServiceCollection();
            serviceCollection.AddSingleton<ITestInterface>(implementationInstance);
            serviceCollection.Decorate<ITestInterface, DecoratingTestImplementation>();

            var serviceProvider = serviceCollection.BuildServiceProvider();

            var firstResolvedInstance = serviceProvider.GetService<ITestInterface>();
            var secondResolvedInstance = serviceProvider.GetService<ITestInterface>();

            firstResolvedInstance.Should().NotBe(secondResolvedInstance);
        }
        
        [Test]
        public void ResolveCorrectChildImplementationForInstanceRegisteredChild()
        {
            var implementationInstance = new TestImplementation();

            var serviceCollection = new ServiceCollection();
            serviceCollection.AddSingleton<ITestInterface>(implementationInstance);
            serviceCollection.Decorate<ITestInterface, DecoratingTestImplementation>();

            var serviceProvider = serviceCollection.BuildServiceProvider();

            var actual = serviceProvider.GetService<ITestInterface>();

            actual
                .Should().BeOfType<DecoratingTestImplementation>()
                .Which
                .InnerTestInterface.Should().Be(implementationInstance);
        }

        [Test]
        public void ResolveDecoratorForInstanceRegisteredChild()
        {
            var serviceCollection = new ServiceCollection();
            serviceCollection.AddSingleton<ITestInterface>(new TestImplementation());
            serviceCollection.Decorate<ITestInterface, DecoratingTestImplementation>();

            var serviceProvider = serviceCollection.BuildServiceProvider();

            var actual = serviceProvider.GetService<ITestInterface>();

            actual.Should().BeOfType<DecoratingTestImplementation>();
        }

        [Test]
        public void ResolveCorrectChildImplementationForFactoryRegisteredChild()
        {
            var testImplementation = new TestImplementation();

            var serviceCollection = new ServiceCollection();
            serviceCollection.AddSingleton<ITestInterface, TestImplementation>(provider => testImplementation);
            serviceCollection.Decorate<ITestInterface, DecoratingTestImplementation>();

            var serviceProvider = serviceCollection.BuildServiceProvider();

            var actual = serviceProvider.GetService<ITestInterface>();

            actual
                .Should().BeOfType<DecoratingTestImplementation>()
                .Which
                .InnerTestInterface.Should().Be(testImplementation);
        }

        [Test]
        public void ResolveDecoratorForFactoryRegisteredChild()
        {
            var serviceCollection = new ServiceCollection();
            serviceCollection.AddSingleton<ITestInterface, TestImplementation>(provider => new TestImplementation());
            serviceCollection.Decorate<ITestInterface, DecoratingTestImplementation>();

            var serviceProvider = serviceCollection.BuildServiceProvider();

            var actual = serviceProvider.GetService<ITestInterface>();

            actual.Should().BeOfType<DecoratingTestImplementation>();
        }

        [Test]
        public void ResolveDecoratorForTypeRegisteredChild()
        {
            var serviceCollection = new ServiceCollection();
            serviceCollection.AddSingleton<ITestInterface, TestImplementation>();
            serviceCollection.Decorate<ITestInterface, DecoratingTestImplementation>();

            var serviceProvider = serviceCollection.BuildServiceProvider();

            var actual = serviceProvider.GetService<ITestInterface>();

            actual.Should().BeOfType<DecoratingTestImplementation>();
        }
    }
}