// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

using Gems.TestInfrastructure.Environment;
using Gems.TestInfrastructure.Integration;

using Moq;

namespace Gems.TestInfrastructure.UnitTests.Integration
{
    public class TestApplicationBuilderExtensionsUseSetting
    {
        [Test]
        public void UseSetting()
        {
            DoTest(
                builder => builder.UseSetting("SettingName", "SettingValue"),
                builder =>
                {
                    builder.Verify(x => x.UseSetting(
                        It.Is("SettingName", StringComparer.Ordinal),
                        It.Is("SettingValue", StringComparer.Ordinal)));
                    builder.VerifyNoOtherCalls();
                });
        }

        [Test]
        public void UseSettingWithObject()
        {
            DoTest(
                builder => builder.UseSetting(
                    "SettingName",
                    new
                    {
                        StringProperty = "StringValue",
                        IntegerProperty = 123,
                        ArrayProperty = new int[] { 1, 2 },
                        ObjectProperty = new
                        {
                            SubProperty = "Test123"
                        },
                    }),
                builder =>
                {
                    builder.Verify(x => x.UseSetting(
                        It.Is("SettingName:StringProperty", StringComparer.Ordinal),
                        It.Is("StringValue", StringComparer.Ordinal)));
                    builder.Verify(x => x.UseSetting(
                        It.Is("SettingName:IntegerProperty", StringComparer.Ordinal),
                        It.Is("123", StringComparer.Ordinal)));
                    builder.Verify(x => x.UseSetting(
                        It.Is("SettingName:ArrayProperty:0", StringComparer.Ordinal),
                        It.Is("1", StringComparer.Ordinal)));
                    builder.Verify(x => x.UseSetting(
                        It.Is("SettingName:ArrayProperty:1", StringComparer.Ordinal),
                        It.Is("2", StringComparer.Ordinal)));
                    builder.Verify(x => x.UseSetting(
                        It.Is("SettingName:ObjectProperty:SubProperty", StringComparer.Ordinal),
                        It.Is("Test123", StringComparer.Ordinal)));
                    builder.VerifyNoOtherCalls();
                });
        }

        [Test]
        public void UseConnectionString()
        {
            DoTest(
                builder => builder.UseConnectionString("NameOfConnection", "ConnectionStringValue"),
                builder =>
                {
                    builder.Verify(x => x.UseSetting(
                        It.Is("ConnectionStrings:NameOfConnection", StringComparer.Ordinal),
                        It.Is("ConnectionStringValue", StringComparer.Ordinal)));
                    builder.VerifyNoOtherCalls();
                });
        }

        [Test]
        public void UseConnectionStringUri()
        {
            DoTest(
                builder => builder.UseConnectionString("UriName", new Uri("http://host:8088/service")),
                builder =>
                {
                    builder.Verify(x => x.UseSetting(
                        It.Is("ConnectionStrings:UriName", StringComparer.Ordinal),
                        It.Is("http://host:8088/service", StringComparer.Ordinal)));
                    builder.VerifyNoOtherCalls();
                });
        }

        [Test]
        public void UseConnectionStringEnvironment()
        {
            var mockDatabaseContainer = new Mock<IDatabaseContainer>();
            mockDatabaseContainer
                .SetupGet(x => x.ConnectionString)
                .Returns("123DatabaseConfigurationName123");
            var databaseContainer = mockDatabaseContainer.Object;

            var mockEnv = new Mock<ITestEnvironment>();
            mockEnv
                .Setup(x => x.Component(It.Is<Type>(obj => obj == typeof(IDatabaseContainer)), It.IsAny<string>()))
                .Returns(databaseContainer);
            var env = mockEnv.Object;

            DoTest(
                builder => builder.UseConnectionString("DatabaseConfigurationName", env),
                builder =>
                {
                    builder.Verify(x => x.UseSetting(
                        It.Is("ConnectionStrings:DatabaseConfigurationName", StringComparer.Ordinal),
                        It.Is("123DatabaseConfigurationName123", StringComparer.Ordinal)));
                    mockEnv.Verify(x => x.Component(
                        It.Is<Type>(obj => obj == typeof(IDatabaseContainer)),
                        It.Is("DatabaseConfigurationName", StringComparer.Ordinal)));
                    mockDatabaseContainer.VerifyGet(x => x.ConnectionString);
                });
        }

        private static void DoTest(
            Action<ITestApplicationBuilder> act,
            Action<Mock<ITestApplicationBuilder>> assert)
        {
            var mockBuilder = new Mock<ITestApplicationBuilder>();
            var builder = mockBuilder.Object;
            act(builder);
            assert(mockBuilder);
        }
    }
}
