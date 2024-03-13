using Gems.TestInfrastructure.Utils;

namespace Gems.TestInfrastructure.Environment
{
    internal class TestEnvironment : AsyncDisposableContainer, ITestEnvironment
    {
        private readonly TestEnvironmentConfiguration configuration;
        private readonly Dictionary<string, object> namedComponents = new Dictionary<string, object>();

        public TestEnvironment(TestEnvironmentConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public object Component(Type type, string name)
            => this.namedComponents[$"{type.FullName}|{name}"];

        public void RegisterComponent(string name, object value, params Type[] types)
        {
            foreach (var type in types)
            {
                this.namedComponents.Add($"{type.FullName}|{name}", value);
            }
        }

        public async Task StartAsync(CancellationToken cancellationToken = default)
        {
            this.configuration.ComponentBuilders.ForEach(create => this.RegisterComponent(create()));

            foreach (var bootstrapAction in this.configuration.Bootstrap)
            {
                await bootstrapAction(this, cancellationToken);
            }
        }
    }
}
