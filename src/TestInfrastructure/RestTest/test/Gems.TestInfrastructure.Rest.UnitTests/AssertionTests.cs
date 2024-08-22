using System.Dynamic;

using FluentAssertions;

using Gems.TestInfrastructure.Rest.Core;
using Gems.TestInfrastructure.Rest.Core.Asserts;

using Newtonsoft.Json.Linq;

namespace Gems.TestInfrastructure.RestTest.UnitTests
{
    internal class AssertionTests
    {
        private TestRunnerContext context;
        private AssertionManager assertionManager;

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

            this.assertionManager = new AssertionManager(this.context);
        }

        [Test]
        public void BooleanAssert()
        {
            var e = Assert.Throws<AssertException>(() => this.assertionManager.Assert("false"));
            e.Expected.Should().Be(true);
            e.Fact.Should().Be(false);
        }

        [Test]
        public void SimpleCompareAssert()
        {
            var e = Assert.Throws<AssertException>(() => this.assertionManager.Assert("1==2"));
            e.Expected.Should().Be(true);
            e.Fact.Should().Be(false);
        }

        [Test]
        public void NonBooleanAssert()
        {
            var e = Assert.Throws<AssertException>(() => this.assertionManager.Assert("null"));
            e.Expected.Should().Be(true);
            e.Fact.Should().BeNull();
        }

        [Test]
        public void AssertNull()
        {
            var response = new ExpandoObject();
            response.TryAdd("body", JObject.Parse(@"{""param"" : ""value"" }"));
            this.context.SetVariable("response", response);
            Assert.Throws<AssertException>(() => this.assertionManager.Assert("Assert.Null(response.body)"));
        }

        [Test]
        public void AssertNotNull()
        {
            var response = new ExpandoObject();
            response.TryAdd("body", null);
            this.context.SetVariable("response", response);
            Assert.Throws<AssertException>(() => this.assertionManager.Assert("Assert.NotNull(response.body)"));
        }

        [Test]
        public void AssertEmpty()
        {
            var nonEmptyList = new List<int>() { 1, 2, 3 };
            this.context.SetVariable("nonEmptyList", nonEmptyList);
            var e = Assert.Throws<AssertException>(() => this.assertionManager.Assert("Assert.Empty(nonEmptyList)"));
            Console.WriteLine(e.Message);
        }
    }
}
