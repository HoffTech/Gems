using FluentAssertions;

using Gems.TestInfrastructure.Utils;

namespace Gems.TestInfrastructure.UnitTests.Utils
{
    public class SettingsHelperFlattenObject
    {
        [Test]
        public void SimpleAnonymousProperty()
        {
            var kvs = SettingsHelper.FlattenObject(123, string.Empty);
            kvs.Should().NotBeNull();
            kvs.Should().HaveCount(1);
            kvs
                .Should()
                .Contain(
                new KeyValuePair<string, string>(string.Empty, "123"));
        }

        [Test]
        public void SimpleIntegerProperty()
        {
            var kvs = SettingsHelper.FlattenObject(123, "Property");
            kvs.Should().NotBeNull();
            kvs.Should().HaveCount(1);
            kvs
                .Should()
                .Contain(
                new KeyValuePair<string, string>("Property", "123"));
        }

        [Test]
        public void SimpleBoolProperty()
        {
            var kvs = SettingsHelper.FlattenObject(true, "Property");
            kvs.Should().NotBeNull();
            kvs.Should().HaveCount(1);
            kvs
                .Should()
                .Contain(
                new KeyValuePair<string, string>("Property", "true"));
        }

        [Test]
        public void SimpleStringProperty()
        {
            var kvs = SettingsHelper.FlattenObject("Hello \"World\"", "Property");
            kvs.Should().NotBeNull();
            kvs.Should().HaveCount(1);
            kvs
                .Should()
                .Contain(
                new KeyValuePair<string, string>("Property", "Hello \"World\""));
        }

        [Test]
        public void SimpleFloatProperty()
        {
            var kvs = SettingsHelper.FlattenObject(1.12300, "Property");
            kvs.Should().NotBeNull();
            kvs.Should().HaveCount(1);
            kvs
                .Should()
                .Contain(
                new KeyValuePair<string, string>("Property", "1.123"));
        }

        [Test]
        public void IntegerArray()
        {
            var kvs = SettingsHelper.FlattenObject(new int[] { 1, 2, 3 }, "Property");
            kvs.Should().NotBeNull();
            kvs.Should().HaveCount(3);
            kvs
                .Should()
                .Contain(
                new KeyValuePair<string, string>[]
                {
                    new KeyValuePair<string, string>("Property:0", "1"),
                    new KeyValuePair<string, string>("Property:1", "2"),
                    new KeyValuePair<string, string>("Property:2", "3"),
                });
        }

        [Test]
        public void BoolArray()
        {
            var kvs = SettingsHelper.FlattenObject(new bool[] { true, false }, "Property");
            kvs.Should().NotBeNull();
            kvs.Should().HaveCount(2);
            kvs
                .Should()
                .Contain(
                new KeyValuePair<string, string>[]
                {
                    new KeyValuePair<string, string>("Property:0", "true"),
                    new KeyValuePair<string, string>("Property:1", "false")
                });
        }

        [Test]
        public void StringArray()
        {
            var kvs = SettingsHelper.FlattenObject(new string[] { "one", "two", "three" }, "Property");
            kvs.Should().NotBeNull();
            kvs.Should().HaveCount(3);
            kvs
                .Should()
                .Contain(
                new KeyValuePair<string, string>[]
                {
                    new KeyValuePair<string, string>("Property:0", "one"),
                    new KeyValuePair<string, string>("Property:1", "two"),
                    new KeyValuePair<string, string>("Property:2", "three"),
                });
        }

        [Test]
        public void ComlexObject()
        {
            var kvs = SettingsHelper.FlattenObject(
                new
                {
                    IntegerProperty = 123,
                    FloatProperty = 1.234,
                    BooleanProperty = true,
                    StringProperty = "Hello \"world\"",
                    ArrayProperty = new int[] { 1, 2, 3 },
                    ObjectProperty = new
                    {
                        Name = "MyObject",
                    },
                    NullProperty = null as object,
                },
                "Property");
            kvs.Should().NotBeNull();
            kvs.Should().HaveCount(8);
            kvs
                .Should()
                .Contain(
                new KeyValuePair<string, string>[]
                {
                    new KeyValuePair<string, string>("Property:IntegerProperty", "123"),
                    new KeyValuePair<string, string>("Property:FloatProperty", "1.234"),
                    new KeyValuePair<string, string>("Property:BooleanProperty", "true"),
                    new KeyValuePair<string, string>("Property:StringProperty", "Hello \"world\""),
                    new KeyValuePair<string, string>("Property:ArrayProperty:0", "1"),
                    new KeyValuePair<string, string>("Property:ArrayProperty:1", "2"),
                    new KeyValuePair<string, string>("Property:ArrayProperty:2", "3"),
                    new KeyValuePair<string, string>("Property:ObjectProperty:Name", "MyObject"),
                });
        }
    }
}
