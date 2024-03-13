namespace Gems.TestInfrastructure.Environment
{
    internal class TestEnvironmentConfiguration
    {
        public List<Func<object>> ComponentBuilders { get; } = new List<Func<object>>();

        public List<Func<ITestEnvironment, CancellationToken, Task>> Bootstrap { get; } = new List<Func<ITestEnvironment, CancellationToken, Task>>();
    }
}
