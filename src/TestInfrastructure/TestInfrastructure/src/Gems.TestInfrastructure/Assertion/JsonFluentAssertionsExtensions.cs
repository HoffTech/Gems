using FluentAssertions.Json;
using FluentAssertions.Primitives;

using Newtonsoft.Json.Linq;

namespace Gems.TestInfrastructure.Assertion
{
    public static class JsonFluentAssertionsExtensions
    {
        public static JTokenAssertions AsJson(this StringAssertions json)
        {
            var jToken = JToken.Parse(json.Subject);
            return jToken.Should();
        }
    }
}
