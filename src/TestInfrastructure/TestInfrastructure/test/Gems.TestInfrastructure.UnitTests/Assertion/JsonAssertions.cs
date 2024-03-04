using FluentAssertions;

using Gems.TestInfrastructure.Assertion;

namespace Gems.TestInfrastructure.UnitTests.Assertion
{
    public class JsonAssertions
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void FormattingDoesNotMatter()
        {
            "{}".Should().AsJson().BeEquivalentTo("{ }");
        }

        [Test]
        public void QuotesDoesNotMatter()
        {
            "{\"property\":\"value\"}".Should().AsJson().BeEquivalentTo("{ 'property' : 'value' }");
        }

        [Test]
        public void PropertyOrderDoesNotMatter()
        {
            @"{
                ""property1"": ""value1"",
                ""property2"": ""value2""
            }"
            .Should()
            .AsJson()
            .BeEquivalentTo(@"{
                ""property2"": ""value2"",
                ""property1"": ""value1""
            }");
        }

        [Test]
        public void ValueOrderMatters()
        {
            "{\"property\":[1,2,3,4,5]}".Should().AsJson().NotBeEquivalentTo("{\"property\":[5,4,3,2,1]}");
        }

        [Test]
        public void SchemaMatters()
        {
            @"{
                ""property1"": ""value1""
            }"
            .Should()
            .AsJson()
            .NotBeEquivalentTo(@"{
                ""property2"": ""value2"",
                ""property1"": ""value1""
            }");
        }
    }
}
