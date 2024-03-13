using FluentAssertions;

using Gems.TestInfrastructure.Utils.Http;

namespace Gems.TestInfrastructure.UnitTests.Utils
{
    public class UriExtensionsTest
    {
        [Test]
        public void Add()
        {
            new Uri("http://host.org/service")
                .Add("method/123")
                .Should()
                .Be(new Uri("http://host.org/service/method/123"));

            new Uri("http://host.org/service/")
                .Add("method/123")
                .Should()
                .Be(new Uri("http://host.org/service/method/123"));

            new Uri("http://host.org/service")
                .Add("/method/123")
                .Should()
                .Be(new Uri("http://host.org/service/method/123"));

            new Uri("http://host.org/service/")
                .Add("/method/123")
                .Should()
                .Be(new Uri("http://host.org/service/method/123"));
        }
    }
}
