// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

using FluentAssertions;

using Gems.TestInfrastructure.Rest.Core.Templates;

namespace Gems.TestInfrastructure.RestTest.UnitTests;

public class MustacheTemplateTests
{
    [Test]
    public void SimpleText()
    {
        var segments = Mustache.Parse("Hello world").ToList();
        segments.Should()
            .ContainSingle(x => x.SegmentType == MustacheSegmentType.Text && x.Value == "Hello world");
    }

    [Test]
    public void SimpleExpression()
    {
        var segments = Mustache.Parse("{{Hello world}}").ToList();
        segments.Should()
            .ContainSingle(x => x.SegmentType == MustacheSegmentType.Expression && x.Value == "Hello world");
    }

    [Test]
    public void InvalidExpression()
    {
        Assert.Throws<ArgumentException>(() => Mustache.Parse("Hello {{ world").ToList());
    }

    [Test]
    public void ComplexExpression()
    {
        var segments = Mustache.Parse("Say {{Hello world}}!").ToList();
        segments.Should()
            .HaveCount(3)
            .And
            .Contain(x => x.SegmentType == MustacheSegmentType.Text && x.Value == "Say ")
            .And
            .Contain(x => x.SegmentType == MustacheSegmentType.Expression && x.Value == "Hello world")
            .And
            .Contain(x => x.SegmentType == MustacheSegmentType.Text && x.Value == "!");
    }
}
