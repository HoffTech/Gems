// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

using System;

using Gems.Http.Serializers;

using NUnit.Framework;

namespace Gems.Http.Tests.Serializers;

public class JsonSerializerHelperTests
{
    [Test]
    public void SerializeObjectToJson_SomePerson_ReturnJson()
    {
        // Arrange
        var obj = new Person
        {
            Name = "Leonid",
            Age = 4,
            Birthday = DateTime.Parse("2020-04-08"),
            IsMale = true
        };

        // Act
        var json = JsonSerializerHelper.SerializeObjectToJson(obj);

        // Assert
        Assert.AreEqual("{\"name\":\"Leonid\",\"age\":4,\"birthday\":\"2020-04-08T00:00:00\",\"isMale\":true}", json);
    }

    [Test]
    public void DeserializeObjectFromJson_SomeJson_ReturnPersonObject()
    {
        // Arrange
        var json = "{\"name\":\"Leonid\",\"age\":4,\"birthday\":\"2020-04-08T00:00:00\",\"isMale\":true}";

        // Act
        var person = JsonSerializerHelper.DeserializeObjectFromJson<Person>(json);

        // Assert
        Assert.NotNull(person);
        Assert.AreEqual(person.Name, "Leonid");
        Assert.AreEqual(person.Age, 4);
        Assert.AreEqual(person.Birthday, DateTime.Parse("2020-04-08"));
        Assert.AreEqual(person.IsMale, true);
    }

    [Test]
    public void SerializeObjectToJson_SomePersonAsStructure_ReturnJson()
    {
        // Arrange
        var obj = new PersonStruct
        {
            Name = "Leonid",
            Age = 4,
            Birthday = DateTime.Parse("2020-04-08"),
            IsMale = true
        };

        // Act
        var json = JsonSerializerHelper.SerializeObjectToJson(obj);

        // Assert
        Assert.AreEqual("{\"name\":\"Leonid\",\"age\":4,\"birthday\":\"2020-04-08T00:00:00\",\"isMale\":true}", json);
    }

    [Test]
    public void DeserializeObjectFromJson_SomeJson_ReturnPersonStructure()
    {
        // Arrange
        var json = "{\"name\":\"Leonid\",\"age\":4,\"birthday\":\"2020-04-08T00:00:00\",\"isMale\":true}";

        // Act
        var person = JsonSerializerHelper.DeserializeObjectFromJson<PersonStruct>(json);

        // Assert
        Assert.NotNull(person);
        Assert.AreEqual(person.Name, "Leonid");
        Assert.AreEqual(person.Age, 4);
        Assert.AreEqual(person.Birthday, DateTime.Parse("2020-04-08"));
        Assert.AreEqual(person.IsMale, true);
    }

    [Test]
    public void SerializeObjectToJson_SomeString_ReturnJson()
    {
        // Arrange
        var name = "Leonid";

        // Act
        var json = JsonSerializerHelper.SerializeObjectToJson(name);

        // Assert
        Assert.AreEqual(name, json);
    }

    [Test]
    public void DeserializeObjectFromJson_SomeString_ReturnString()
    {
        // Arrange
        var json = "Leonid";

        // Act
        var name = JsonSerializerHelper.DeserializeObjectFromJson<string>(json);

        // Assert
        Assert.AreEqual(json, name);
    }

    [Test]
    public void DeserializeObjectFromJson_SomeStringWithQuotes_ReturnString()
    {
        // Arrange
        var json = "\"Leonid\"";

        // Act
        var name = JsonSerializerHelper.DeserializeObjectFromJson<string>(json);

        // Assert
        Assert.AreEqual(json.Trim('"'), name);
    }

    [Test]
    public void DeserializeObjectFromJson_SomeStringJson_ReturnString()
    {
        // Arrange
        var json = "{\"name\":\"Leonid\",\"age\":4,\"birthday\":\"2020-04-08T00:00:00\",\"isMale\":true}";

        // Act
        var name = JsonSerializerHelper.DeserializeObjectFromJson<string>(json);

        // Assert
        Assert.AreEqual(json, name);
    }

    [Test]
    public void SerializeObjectToJson_SomeDateTimeAsString_ReturnJson()
    {
        // Arrange
        var dateTime = DateTime.Parse("2020-04-08");

        // Act
        var json = JsonSerializerHelper.SerializeObjectToJson(dateTime);

        // Assert
        Assert.AreEqual($"\"{dateTime:yyyy-MM-ddTHH:ss:mm}\"", json);
    }

    [Test]
    public void DeserializeObjectFromJson_SomeDateTimeAsString_ReturnDateTime()
    {
        // Arrange
        var json = "2020-04-08T00:00:00";

        // Act
        var dateTime = JsonSerializerHelper.DeserializeObjectFromJson<DateTime>(json);

        // Assert
        Assert.AreEqual(json, $"{dateTime:yyyy-MM-ddTHH:ss:mm}");
    }

    [Test]
    public void DeserializeObjectFromJson_SomeDateTimeWithQuotes_ReturnDateTime()
    {
        // Arrange
        var json = "\"2020-04-08T00:00:00\"";

        // Act
        var dateTime = JsonSerializerHelper.DeserializeObjectFromJson<DateTime>(json);

        // Assert
        Assert.AreEqual(json.Trim('"'), $"{dateTime:yyyy-MM-ddTHH:ss:mm}");
    }
}
