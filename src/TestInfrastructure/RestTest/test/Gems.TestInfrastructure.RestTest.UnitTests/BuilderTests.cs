using FluentAssertions;

using Gems.TestInfrastructure.RestTest.Builders;

using Newtonsoft.Json;

namespace Gems.TestInfrastructure.RestTest.UnitTests
{
    public class BuilderTests
    {
        private TestRunnerContext context;

        [OneTimeSetUp]
        public void Setup()
        {
            this.context = new TestRunnerContext(new TestRunnerContextOptions()
            {
                Namespaces = new List<string>
                {
                    "System",
                },
                Faker = new TestRunnerContextFakerOptions()
                {
                    Locale = "ru",
                },
            });
        }

        [Test]
        public void ParseTest()
        {
            var test = JsonConvert.DeserializeObject<Model.Test>(File.ReadAllText("Resources/Tests/pets-post.json"));
            test.Should().NotBeNull();
        }

        [TestCase("post", "POST")]
        [TestCase("{{\"POST\"}}", "POST")]
        [TestCase("{{Fake.OneOf(\"delete\", \"DELETE\")}}", "DELETE")]
        [TestCase("", "GET")]
        [TestCase(null, "GET")]
        public void HttpMethodParse(string method, string expectedMethodString)
        {
            var expectedMethod = HttpMethod.Parse(expectedMethodString);
            var builder = new MethodBuilder(this.context);
            var factMethod = builder.Build(method);
            factMethod.Should().Be(expectedMethod);
        }
    }
}
