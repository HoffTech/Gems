// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

using System;

using NUnit.Framework;

namespace Gems.Text.Json.Tests.Converters;

public class DateOnlyJsonConverterTests
{
    [Test]
    public void ShouldSerializeToDateOnly()
    {
        var dateNow = DateTime.Now;
        var data = new DateOnlyHolder
        {
            DateOnly = DateOnly.FromDateTime(dateNow)
        };

        var serializedData = data.Serialize();
        Assert.AreEqual($"{{\"DateOnly\":\"{data.DateOnly.ToString("O")}\"}}", serializedData);
    }

    [Test]
    public void ShouldDeserializeDateOnly()
    {
        var dateNow = DateTime.Now;
        var dateOnly = DateOnly.FromDateTime(dateNow);
        var serializedData = $"{{\"DateOnly\":\"{dateOnly.ToString("O")}\"}}";
        var data = serializedData.Deserialize<DateOnlyHolder>();

        Assert.NotNull(data);
        Assert.AreEqual(dateOnly, data.DateOnly);
    }
}
