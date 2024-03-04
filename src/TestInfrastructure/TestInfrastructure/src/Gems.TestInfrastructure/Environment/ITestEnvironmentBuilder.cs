namespace Gems.TestInfrastructure.Environment
{
    public interface ITestEnvironmentBuilder
    {
        ITestEnvironment Build();

        Task<ITestEnvironment> BuildAsync(CancellationToken cancellationToken = default);

        ITestEnvironmentBuilder UseBootstraper(Func<ITestEnvironment, CancellationToken, Task> bootstrap);

        ITestEnvironmentBuilder UseComponent<TComponent>(Func<TComponent> build) where TComponent : class;

        ITestEnvironmentBuilder UseComponent<TComponent>(Action<TComponent> setup) where TComponent : class;
    }
}
