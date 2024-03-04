namespace Gems.TestInfrastructure.Environment
{
    public static class TestEnvironmentExtensions
    {
        public static T Component<T>(this ITestEnvironment env, string name)
            => (T)env.Component(typeof(T), name);

        public static void RegisterComponent<T>(this ITestEnvironment env, string name, object value)
            => env.RegisterComponent(name, value, typeof(T));

        public static void RegisterComponent(this ITestEnvironment env, string name, object value)
            => env.RegisterComponent(name, value, value.GetType());

        public static IDatabaseContainer Database(this ITestEnvironment env, string name)
            => env.Component<IDatabaseContainer>(name);

        public static string DatabaseConnectionString(this ITestEnvironment env, string name)
            => env.Component<IDatabaseContainer>(name).ConnectionString;
    }
}
