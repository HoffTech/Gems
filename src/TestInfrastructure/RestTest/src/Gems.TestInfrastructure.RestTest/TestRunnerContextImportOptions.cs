namespace Gems.TestInfrastructure.RestTest;

public class TestRunnerContextImportOptions
{
    public TestRunnerContextImportOptions(Type type, string name = default)
    {
        this.Type = type;
        this.Name = name;
    }

    public Type Type { get; set; }

    public string Name { get; set; }
}
