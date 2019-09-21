using System;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using Microsoft.Extensions.DependencyInjection;
using UnOfficial.Microsoft.Extensions.DependencyInjection.Decorator.Tests;

namespace Benchmarking
{
    public class Program
    {
        static void Main(string[] args)
        {
            BenchmarkRunner.Run<DecoratorVsNormal>();
        }
    }

    [MinIterationCount(200)]
    [MaxIterationCount(250)]
    public class DecoratorVsNormal
    {
        
        private IServiceProvider _nonDecoratedServiceProvider;
        private IServiceProvider _decoratedServiceProvider;
        
        
        [GlobalSetup]
        public void Setup()
        {
            _nonDecoratedServiceProvider = CreateNonDecoratedServiceProvider();
            _decoratedServiceProvider = CreateDecoratedServiceProvider();
        }

        private IServiceProvider CreateDecoratedServiceProvider() => new ServiceCollection()
            .AddTransient<ITestInterface, TestImplementation>()
            .Decorate<ITestInterface, DecoratingTestImplementation>()
            .BuildServiceProvider();
        
        private IServiceProvider CreateNonDecoratedServiceProvider() => new ServiceCollection()
            .AddTransient<ITestInterface, TestImplementation>()
            .BuildServiceProvider();

        [Benchmark]
        public void ResolveDecorated() => _decoratedServiceProvider.GetService<ITestInterface>();
        
        [Benchmark]
        public void ResolveNonDecorated() => _nonDecoratedServiceProvider.GetService<ITestInterface>();

    }
}