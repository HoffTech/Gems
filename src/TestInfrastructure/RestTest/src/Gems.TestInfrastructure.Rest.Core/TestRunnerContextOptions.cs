namespace Gems.TestInfrastructure.Rest.Core;

public class TestRunnerContextOptions
{
    public List<string> Namespaces { get; set; }

    public TestRunnerContextFakerOptions Faker { get; set; }
}
