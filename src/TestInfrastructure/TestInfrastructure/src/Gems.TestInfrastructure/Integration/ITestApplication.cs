namespace Gems.TestInfrastructure.Integration
{
    public interface ITestApplication : IDisposable, IAsyncDisposable
    {
        public HttpClient HttpClient { get; }
    }
}
