// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

using Gems.Text.Json.Converters;

using NUnit.Framework;

namespace Gems.Text.Json.Tests.Converters;

public class UnspecifiedDateTimeConverterTests
{
    [Test]
    public void ShouldDeserializeUnspecifiedToMoscow()
    {
        var value = DateTime.SpecifyKind(DateTime.UtcNow.AddHours(+3), DateTimeKind.Unspecified);
        var deserializerFormat = @"yyyy-MM-ddTHH:mm:ss.fffffff";
        var expectedValue = DateTime.SpecifyKind(value, DateTimeKind.Utc).AddHours(-3);

        var data = $"{{\"value\":\"{value.ToString(deserializerFormat)}\"}}";
        var deserializedData = data.Deserialize<DateTimeValueHolder>(new List<JsonConverter> { new UnspecifiedDateTimeConverter() { DeserializerTimeZone = "Europe/Moscow", DeserializerFormat = deserializerFormat } });
        Assert.NotNull(deserializedData);
        Assert.AreEqual(expectedValue, deserializedData.Value);
    }

    [Test]
    public void ShouldSerializeUnspecifiedToMoscow()
    {
        var data = new DateTimeValueHolder
        {
            Value = DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Unspecified)
        };
        var targetSerializerFormat = @"yyyy-MM-ddTHH:mm:ss\\u002B03:00";
        var expectedValue = DateTime.SpecifyKind(data.Value, DateTimeKind.Local).ToUniversalTime().AddHours(+3).ToString(targetSerializerFormat);

        var serializedData = data.Serialize(new List<JsonConverter> { new UnspecifiedDateTimeConverter() { SerializerTimeZone = "Europe/Moscow" } }, camelCase: true);
        Assert.AreEqual($"{{\"value\":\"{expectedValue}\"}}", serializedData);
    }

    [Test]
    public void ShouldSerializeUnspecifiedAsLocalToMoscow()
    {
        var data = new DateTimeValueHolder
        {
            Value = DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Unspecified)
        };
        var targetSerializerFormat = @"yyyy-MM-ddTHH:mm:ss\\u002B03:00";
        var expectedValue = DateTime.SpecifyKind(data.Value, DateTimeKind.Local).ToUniversalTime().AddHours(+3).ToString(targetSerializerFormat);

        var serializedData = data.Serialize(new List<JsonConverter> { new UnspecifiedDateTimeConverter() { SerializerTimeZone = "Europe/Moscow", TargetTimeKind = DateTimeKind.Local } }, camelCase: true);
        Assert.AreEqual($"{{\"value\":\"{expectedValue}\"}}", serializedData);
    }

    [Test]
    public void ShouldSerializeUnspecifiedAsUtcToMoscow()
    {
        var data = new DateTimeValueHolder
        {
            Value = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Unspecified)
        };
        var targetSerializerFormat = @"yyyy-MM-ddTHH:mm:ss\\u002B03:00";
        var expectedValue = DateTime.SpecifyKind(data.Value, DateTimeKind.Utc).AddHours(+3).ToString(targetSerializerFormat);

        var serializedData = data.Serialize(new List<JsonConverter> { new UnspecifiedDateTimeConverter() { SerializerTimeZone = "Europe/Moscow", TargetTimeKind = DateTimeKind.Utc } }, camelCase: true);
        Assert.AreEqual($"{{\"value\":\"{expectedValue}\"}}", serializedData);
    }

    [Test]
    public void ShouldSerializeUtcToMoscow()
    {
        var data = new DateTimeValueHolder
        {
            Value = DateTime.UtcNow
        };
        var targetSerializerFormat = @"yyyy-MM-ddTHH:mm:ss\\u002B03:00";
        var expectedValue = data.Value.AddHours(+3).ToString(targetSerializerFormat);

        var serializedData = data.Serialize(new List<JsonConverter> { new UnspecifiedDateTimeConverter() { SerializerTimeZone = "Europe/Moscow" } }, camelCase: true);
        Assert.AreEqual($"{{\"value\":\"{expectedValue}\"}}", serializedData);
    }

    [Test]
    public void ShouldSerializeLocalToMoscow()
    {
        var data = new DateTimeValueHolder
        {
            Value = DateTime.Now
        };
        var targetSerializerFormat = @"yyyy-MM-ddTHH:mm:ss\\u002B03:00";
        var expectedValue = data.Value.ToUniversalTime().AddHours(+3).ToString(targetSerializerFormat);

        var serializedData = data.Serialize(new List<JsonConverter> { new UnspecifiedDateTimeConverter() { SerializerTimeZone = "Europe/Moscow" } }, camelCase: true);
        Assert.AreEqual($"{{\"value\":\"{expectedValue}\"}}", serializedData);
    }

    [Test]
    public void T2()
    {
        var deserializedData = "{\"value1\":123.4567,\"value2\":123.4567,\"value3\":\"\",\"value4\":\"2024-05-21T03:18:54\"}".Deserialize<ValuesHolder>();
        Console.WriteLine(deserializedData.Value1);
        Console.WriteLine(deserializedData.Value2);
        Console.WriteLine(deserializedData.Value3);
        Console.WriteLine(deserializedData.Value4.ToString("yyyy-MM-ddTHH:mm:sszzz"));
    }

    private class DateTimeValueHolder
    {
        public DateTime Value { get; set; }
    }

    private class ValuesHolder
    {
        public double Value1 { get; set; }

        public float Value2 { get; set; }

        [EmptyStringToNullConverter]
        public string Value3 { get; set; }

        [UnspecifiedDateTimeConverter(DeserializerTimeZone = "Europe/Moscow")]
        public DateTime Value4 { get; set; }
    }
}
