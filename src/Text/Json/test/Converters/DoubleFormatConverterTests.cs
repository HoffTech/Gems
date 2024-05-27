// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

using System.Collections.Generic;
using System.Text.Json.Serialization;

using Gems.Text.Json.Converters;

using NUnit.Framework;

namespace Gems.Text.Json.Tests.Converters;

public class DoubleRoundConverterTests
{
    [TestCase(123.45678, 1)]
    [TestCase(123.45678, 2)]
    [TestCase(123.45678, 3)]
    [TestCase(123.45678, 4)]
    public void ShouldSerializeWithSpecifyDecimalDigitsLength(double value, int decimalDigitsLength)
    {
        var data = new DoubleValueHolder
        {
            Value = value
        };

        var serializedData = data.Serialize(new List<JsonConverter> { new DoubleRoundConverter { DecimalDigitsLength = decimalDigitsLength } }, camelCase: true);
        Assert.AreEqual($"{{\"value\":{data.Value.ToString($"F{decimalDigitsLength}")}}}", serializedData);
    }

    [TestCase("123.4")]
    [TestCase("123.45")]
    [TestCase("123.456")]
    [TestCase("123.4567")]
    public void ShouldDeserializeToDouble(string value)
    {
        var serializedData = $"{{\"value\":{value}}}";
        var data = serializedData.Deserialize<DoubleValueHolder>();

        Assert.NotNull(data);
    }

    private class DoubleValueHolder
    {
        public double Value { get; set; }
    }
}
