namespace Gems.TestInfrastructure.RestTest.Model;

public class TestRequest
{
    public string Method { get; set; }

    public string Url { get; set; }

    public object Body { get; set; }

    public Dictionary<string, string> Headers { get; set; }

    public string Timeout { get; set; }
}
