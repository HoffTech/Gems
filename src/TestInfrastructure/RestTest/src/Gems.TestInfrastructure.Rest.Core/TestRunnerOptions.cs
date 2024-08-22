namespace Gems.TestInfrastructure.Rest.Core;

public class TestRunnerOptions
{
    public TestRunnerContextOptions Context { get; set; }

    public HttpClient HttpClient { get; set; }

    public static TestRunnerOptions CreateDefault()
    {
        return new TestRunnerOptions
        {
            Context = new TestRunnerContextOptions
            {
                Namespaces = new List<string> { "System", },
                Faker = new TestRunnerContextFakerOptions() { Locale = "ru", },
            }
        };
    }
}
