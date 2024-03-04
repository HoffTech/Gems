namespace Gems.TestInfrastructure.Environment
{
    public class TestEnvironmentBuilder : ITestEnvironmentBuilder
    {
        private readonly TestEnvironmentConfiguration configuration;

        public TestEnvironmentBuilder()
        {
            this.configuration = new TestEnvironmentConfiguration();
        }

        public async Task<ITestEnvironment> BuildAsync(CancellationToken cancellationToken = default)
        {
            var env = new TestEnvironment(this.configuration);
            await env.StartAsync(cancellationToken);
            return env;
        }

        public ITestEnvironment Build()
        {
            var env = new TestEnvironment(this.configuration);
            env
                .StartAsync()
                .ConfigureAwait(false)
                .GetAwaiter()
                .GetResult();
            return env;
        }

        public ITestEnvironmentBuilder UseComponent<TComponent>(Func<TComponent> build)
            where TComponent : class
        {
            this.configuration.ComponentBuilders.Add(build);
            return this;
        }

        public ITestEnvironmentBuilder UseComponent<TComponent>(Action<TComponent> setup)
            where TComponent : class
        {
            this.configuration.ComponentBuilders.Add(() =>
            {
                var component = Activator.CreateInstance<TComponent>();
                setup?.Invoke(component);
                return component;
            });
            return this;
        }

        public ITestEnvironmentBuilder UseBootstraper(Func<ITestEnvironment, CancellationToken, Task> bootstraper)
        {
            this.configuration.Bootstrap.Add(bootstraper);
            return this;
        }
    }
}
