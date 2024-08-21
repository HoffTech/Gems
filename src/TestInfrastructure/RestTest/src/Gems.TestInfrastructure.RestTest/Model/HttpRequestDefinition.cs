namespace Gems.TestInfrastructure.RestTest.Model;

public class HttpRequestDefinition
{
    public HttpMethod Method { get; set; }

    public Uri Address { get; set; }

    public object Body { get; set; }

    public Dictionary<string, string> Headers { get; set; }

    public TimeSpan? Timeout { get; set; }
}