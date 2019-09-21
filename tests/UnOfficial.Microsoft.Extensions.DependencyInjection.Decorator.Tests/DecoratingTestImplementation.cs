namespace UnOfficial.Microsoft.Extensions.DependencyInjection.Decorator.Tests
{
    public class DecoratingTestImplementation : ITestInterface
    {
        public DecoratingTestImplementation(ITestInterface innerTestInterface)
        {
            InnerTestInterface = innerTestInterface;
        }

        public ITestInterface InnerTestInterface { get; }
    }
    
    public class DecoratingTestImplementationWithDependencies : ITestInterface
    {

        public DecoratingTestImplementationWithDependencies(ITestInterface innerTestInterface, ITestDependency testDependency)
        {
            TestDependency = testDependency;
            InnerTestInterface = innerTestInterface;
        }

        public ITestInterface InnerTestInterface { get; }
        
        public ITestDependency TestDependency { get; }
    }

    public interface ITestDependency {}

    public class TestDependency : ITestDependency {}
}