// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

using System.Collections.Generic;
using System.Text.Json.Serialization;

using Gems.Text.Json.Converters;

using NUnit.Framework;

namespace Gems.Text.Json.Tests.Converters;

public class FloatRoundConverterTests
{
    [TestCase(123.45678F, 1)]
    [TestCase(123.45678F, 2)]
    [TestCase(123.45678F, 3)]
    [TestCase(123.45678F, 4)]
    public void ShouldSerializeWithSpecifyDecimalDigitsLength(float value, int decimalDigitsLength)
    {
        var data = new FloatValueHolder
        {
            Value = value
        };

        var serializedData = data.Serialize(new List<JsonConverter> { new FloatRoundConverter { DecimalDigitsLength = decimalDigitsLength } }, camelCase: true);
        Assert.AreEqual($"{{\"value\":{data.Value.ToString($"F{decimalDigitsLength}")}}}", serializedData);
    }

    [TestCase("123.4")]
    [TestCase("123.45")]
    [TestCase("123.456")]
    [TestCase("123.4567")]
    public void ShouldDeserializeToDouble(string value)
    {
        var serializedData = $"{{\"value\":{value}}}";
        var data = serializedData.Deserialize<FloatValueHolder>(new List<JsonConverter> { new FloatRoundConverter() });

        Assert.NotNull(data);
    }

    private class FloatValueHolder
    {
        public float Value { get; set; }
    }
}
