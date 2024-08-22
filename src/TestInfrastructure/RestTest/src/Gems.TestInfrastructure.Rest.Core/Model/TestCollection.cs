namespace Gems.TestInfrastructure.Rest.Core.Model;

public class TestCollection : TestScope
{
    public string Name { get; set; }

    public string Description { get; set; }

    public string Author { get; set; }

    public List<Test> Tests { get; set; }
}
