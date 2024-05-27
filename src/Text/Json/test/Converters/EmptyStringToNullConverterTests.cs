// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

using System.Collections.Generic;
using System.Text.Json.Serialization;

using Gems.Text.Json.Converters;

using NUnit.Framework;

namespace Gems.Text.Json.Tests.Converters;

public class EmptyStringToNullConverterTests
{
    [Test]
    public void ShouldSerializeEmptyStringToNull()
    {
        var data = new StringValueHolder
        {
            Value = string.Empty
        };

        var serializedData = data.Serialize(new List<JsonConverter> { new EmptyStringToNullConverter() }, camelCase: true);
        Assert.AreEqual($"{{\"value\":null}}", serializedData);
    }

    [Test]
    public void ShouldDeserializeEmptyStringToNull()
    {
        var serializedData = $"{{\"value\":\"\"}}";
        var data = serializedData.Deserialize<StringValueHolder>(new List<JsonConverter> { new EmptyStringToNullConverter() });

        Assert.NotNull(data);
        Assert.Null(data.Value);
    }

    private class StringValueHolder
    {
        public string Value { get; set; }
    }
}
