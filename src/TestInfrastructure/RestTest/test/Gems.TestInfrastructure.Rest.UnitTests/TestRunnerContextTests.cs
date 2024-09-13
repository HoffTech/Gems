using FluentAssertions;

using Gems.TestInfrastructure.Rest.Core;

using Newtonsoft.Json;

namespace Gems.TestInfrastructure.RestTest.UnitTests;

public class TestRunnerContextTests
{
    private TestRunnerContext context;

    [OneTimeSetUp]
    public void Setup()
    {
        this.context = new TestRunnerContext(new TestRunnerContextOptions()
        {
            Namespaces = new List<string>
            {
                "System"
            },
            Faker = new TestRunnerContextFakerOptions()
            {
                Locale = "ru",
            },
        });
    }

    [Test]
    public void ParseJson()
    {
        var o = Newtonsoft.Json.JsonConvert.DeserializeObject<object>("{ \"setId\": \"123\" }");
        Console.WriteLine(o.ToString());
    }

    [Test]
    public void AddExpression()
    {
        var a = this.context.Eval("{{1 + 1}}");
        a.Should().Be(2);
    }

    [Test]
    public void Text()
    {
        var a = this.context.Eval("Hello world");
        a.Should().Be("Hello world");
    }

    [Test]
    public void BooleanTrue()
    {
        var a = this.context.Eval("{{true}}");
        a.Should().Be(true);
    }

    [Test]
    public void BooleanFalse()
    {
        var a = this.context.Eval("{{false}}");
        a.Should().Be(false);
    }

    [Test]
    public void Integer()
    {
        var a = this.context.Eval("{{123}}");
        a.Should().Be(123);
    }

    [Test]
    public void Double()
    {
        var a = this.context.Eval("{{123.56}}");
        a.Should().Be(123.56);
    }

    [Test]
    public void Variables()
    {
        this.context.SetVariable("a", 2);
        this.context.SetVariable("b", 8);
        var c = this.context.Eval("{{a + b}}");
        c.Should().Be(10);
        this.context.SetVariable("c", c);
        var d = this.context.Eval("{{c * c}}");
        d.Should().Be(100);
    }

    [Test]
    public void Template()
    {
        this.context.SetVariable("a", 123456);
        this.context.SetTemplateVariable("b", "{{a}}");
        var d = this.context.Eval("{{b}}");
        d.Should().Be(123456);
        this.context.SetVariable("a", 654321);
        d = this.context.Eval("{{b}}");
        d.Should().Be(654321);
    }

    [Test]
    public void ComplexTemplate()
    {
        this.context.SetTemplateVariable("a", "{{Fake.Long()}}");
        var b = this.context.Eval("{{a}}, {{a}}, {{a}}");
        (b as string).Should().MatchRegex(@"-?\d+, -?\d+, -?\d+");
        var c = this.context.Eval("{{a.ToArray(3)}}");
        (c as object[]).Should().HaveCount(3);
    }

    [Test]
    public void DoubleVariable()
    {
        this.context.SetVariable("DoubleVar", 123.56);
        var a = this.context.Eval("{{DoubleVar}}");
        a.Should().Be(123.56);
    }

    [Test]
    public void IntegerVariable()
    {
        this.context.SetVariable("IntegerVar", 123);
        var a = this.context.Eval("{{IntegerVar}}");
        a.Should().Be(123);
    }

    [Test]
    public void TextVariable()
    {
        this.context.SetVariable("TextVar", "Hello world");
        var a = this.context.Eval("{{TextVar}}");
        a.Should().Be("Hello world");
    }

    [Test]
    public void ComplexText()
    {
        this.context.SetVariable("TextVar", "Hello world");
        this.context.SetVariable("IntegerVar", 123);
        this.context.SetVariable("TrueVar", true);
        var a = this.context.Eval("Text={{TextVar}}, Int={{IntegerVar}}, True={{TrueVar}}");
        a.Should().Be("Text=Hello world, Int=123, True=True");
    }

    [Test]
    public void TimeSpanZero()
    {
        var a = this.context.ParseTimeSpan("0");
        a.Should().Be(TimeSpan.Zero);
    }

    [Test]
    public void TimeSpanNull()
    {
        var a = this.context.ParseTimeSpan(string.Empty);
        a.Should().BeNull();
    }

    [Test]
    public void TimeSpanInfinite()
    {
        var a = this.context.ParseTimeSpan("inf");
        a.Should().Be(TimeSpan.MaxValue);
    }

    [Test]
    public void TimeSpanParse()
    {
        this.context.ParseTimeSpan("01:30:00").Should().Be(TimeSpan.FromMinutes(90));
        this.context.ParseTimeSpan("3h").Should().Be(TimeSpan.FromHours(3));
        this.context.ParseTimeSpan("15m").Should().Be(TimeSpan.FromMinutes(15));
        this.context.ParseTimeSpan("30s").Should().Be(TimeSpan.FromSeconds(30));
        this.context.ParseTimeSpan("500ms").Should().Be(TimeSpan.FromMilliseconds(500));
    }

    [Test]
    public void FunctionMath()
    {
        this.context.Eval("{{Math.Round(Math.Abs(-123.456))}}").Should().Be(123.0);
    }

    [Test]
    public void FakeEmail()
    {
        this.context.Eval("{{Fake.Email()}}")
            .ToString()
            .Should()
            .MatchRegex(@"^.+@.+\..+$");
    }

    [Test]
    public void FakePassword()
    {
        this.context.Eval("{{Fake.Password(12)}}")
            .ToString()
            .Should()
            .MatchRegex(@"^\w{12}$");
    }

    [Test]
    public void FunctionGuid()
    {
        this.context.Eval("{{Guid.NewGuid()}}")
            .ToString()
            .Should()
            .MatchRegex(@"^[0-9abcdef]{8}-[0-9abcdef]{4}-[0-9abcdef]{4}-[0-9abcdef]{4}-[0-9abcdef]{12}$");
    }

    [Test]
    public void EvalPetsPost()
    {
        var json = JsonConvert.DeserializeObject<Rest.Core.Model.Test>(File.ReadAllText("Resources/Tests/pets-post.json"));
        var body = this.context.Eval(json.Request.Body);
        body.Should().NotBeNull();
        var dynBody = body as dynamic;
        (dynBody.name as string).Should().Be("cat");
        (dynBody.say as string).Should().Be("meow");
    }

    [Test]
    public void EvalPetsPostGeneric()
    {
        var json = JsonConvert.DeserializeObject<Rest.Core.Model.Test>(File.ReadAllText("Resources/Tests/pets-post-generic.json"));
        var body = this.context.Eval(json.Request.Body);
        body.Should().NotBeNull();
        var dynBody = body as dynamic;
        (dynBody.name as string).Should().BeOneOf("cat", "dog", "cow");
        (dynBody.say as string).Should().BeOneOf("meow", "woof", "moo");
        (dynBody.friends as List<object>).Should().HaveCount(2);
        (dynBody.friends as List<object>)[0].Should().BeOneOf("cat", "dog", "cow");
        (dynBody.friends as List<object>)[1].Should().BeOneOf("cat", "dog", "cow");
    }

    [Test]
    public void Scope()
    {
        this.context.SetVariable("cat", "meow");
        (this.context.Eval("{{cat}}") as string).Should().Be("meow");
        this.context.PushScope();
        this.context.SetVariable("cat", "woof");
        (this.context.Eval("{{cat}}") as string).Should().Be("woof");
        this.context.PopScope();
        (this.context.Eval("{{cat}}") as string).Should().Be("meow");
    }
}
