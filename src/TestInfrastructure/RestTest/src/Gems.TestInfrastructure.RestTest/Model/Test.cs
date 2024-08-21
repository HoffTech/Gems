namespace Gems.TestInfrastructure.RestTest.Model;

public class Test : TestScope
{
    public string Name { get; set; }

    public string Description { get; set; }

    public string Author { get; set; }

    public TestRequest Request { get; set; }

    public List<object> Asserts { get; set; }

    public List<KeyValuePair<string, object>> Output { get; set; }
}
