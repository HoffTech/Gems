namespace Gems.TestInfrastructure.Environment
{
    public interface ITestEnvironment : IDisposable, IAsyncDisposable
    {
        object Component(Type type, string name);

        void RegisterComponent(string name, object value, params Type[] types);
    }
}
