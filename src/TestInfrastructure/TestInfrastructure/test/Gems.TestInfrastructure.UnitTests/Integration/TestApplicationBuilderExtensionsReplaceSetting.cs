using System.Text.RegularExpressions;

using FluentAssertions;

using Gems.TestInfrastructure.Integration;

using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;

using Moq;

namespace Gems.TestInfrastructure.UnitTests.Integration
{
    public class TestApplicationBuilderExtensionsReplaceSetting
    {
        [Test]
        public void ConfigureAppConfiguration()
        {
            DoTest(
                builder =>
                {
                    builder.ConfigureAppConfiguration((ctx, cb) => ctx.Configuration["MyKey"] = "SimpleValue");
                },
                configuration =>
                {
                    configuration
                        .GetValue<string>("MyKey")
                        .Should()
                        .Be("SimpleValue");
                });
        }

        [Test]
        public void ReplaceSettingWithStringPathAndStringValue()
        {
            DoTest(
                builder =>
                {
                    builder.ReplaceSetting("AllowedHosts", "*.myhost.com");
                },
                configuration =>
                {
                    configuration
                        .GetValue<string>("AllowedHosts")
                        .Should()
                        .Be("*.myhost.com");
                });
        }

        [Test]
        public void ReplaceSettingWithStringPathAndStringValueOrdinalCompare()
        {
            DoTest(
                builder =>
                {
                    builder.ReplaceSetting("allowedhosts", "*.myhost.com");
                },
                configuration =>
                {
                    configuration
                        .GetValue<string>("AllowedHosts")
                        .Should()
                        .Be("*");
                });
        }

        [Test]
        public void ReplaceSettingWithStringPathAndStringValueIgnoreCase()
        {
            DoTest(
                builder =>
                {
                    builder.ReplaceSetting("allowedhosts", StringComparison.InvariantCultureIgnoreCase, "*.myhost.com");
                },
                configuration =>
                {
                    configuration
                        .GetValue<string>("AllowedHosts")
                        .Should()
                        .Be("*.myhost.com");
                });
        }

        [Test]
        public void ReplaceSettingWithPathMatcherAndValueFactory()
        {
            DoTest(
                builder =>
                {
                    builder.ReplaceSetting(
                        path => path.Equals("allowedhosts", StringComparison.InvariantCultureIgnoreCase),
                        oldValue => "*.myhost.com");
                },
                configuration =>
                {
                    configuration
                        .GetValue<string>("AllowedHosts")
                        .Should()
                        .Be("*.myhost.com");
                });
        }

        [Test]
        public void ReplaceSettingWithRegexPathAndStringValue()
        {
            DoTest(
                builder =>
                {
                    builder.ReplaceSetting(
                        new Regex(@"^Jobs:Triggers:[^:]+$"),
                        "0 0 1 * * 2099");
                },
                configuration =>
                {
                    configuration
                        .GetValue<string>("Jobs:Triggers:LoadGoods")
                        .Should()
                        .Be("0 0 1 * * 2099");
                    configuration
                        .GetValue<string>("Jobs:Triggers:LoadGoodsAttributes")
                        .Should()
                        .Be("0 0 1 * * 2099");
                    configuration
                        .GetValue<string>("Jobs:Triggers:LoadSales")
                        .Should()
                        .Be("0 0 1 * * 2099");
                });
        }

        [Test]
        public void ReplaceSettingWithRegexPathAndValueFactory()
        {
            DoTest(
                builder =>
                {
                    builder.ReplaceSetting(
                        new Regex(@"^Jobs:Triggers:[^:]+$"),
                        oldValue => Regex.Replace(oldValue, @"\s[^\s]+$", " 2099"));
                },
                configuration =>
                {
                    configuration
                        .GetValue<string>("Jobs:Triggers:LoadGoods")
                        .Should()
                        .Be("0 0 0 * * 2099");
                    configuration
                        .GetValue<string>("Jobs:Triggers:LoadGoodsAttributes")
                        .Should()
                        .Be("0 45 0 * * 2099");
                    configuration
                        .GetValue<string>("Jobs:Triggers:LoadSales")
                        .Should()
                        .Be("0 0 1 * * 2099");
                });
        }

        [Test]
        public void ReplaceSettingWithStringPathAndValueFactory()
        {
            DoTest(
                builder =>
                {
                    builder.ReplaceSetting("AllowedHosts", oldValue => $"{oldValue}.myhost.com");
                },
                configuration =>
                {
                    configuration
                        .GetValue<string>("AllowedHosts")
                        .Should()
                        .Be("*.myhost.com");
                });
        }

        [Test]
        public void ReplaceSettingWithStringPathAndValueFactoryIgnoreCase()
        {
            DoTest(
                builder =>
                {
                    builder.ReplaceSetting("allowedhosts", StringComparison.InvariantCultureIgnoreCase, oldValue => $"{oldValue}.myhost.com");
                },
                configuration =>
                {
                    configuration
                        .GetValue<string>("AllowedHosts")
                        .Should()
                        .Be("*.myhost.com");
                });
        }

        private static void DoTest(
            Action<ITestApplicationBuilder> act,
            Action<IConfiguration> assert)
        {
            var configurationBuilder = new ConfigurationBuilder();
            configurationBuilder.AddJsonFile(@"Resources/sampleSettings.json");
            var configuration = configurationBuilder.Build();
            var webHostBuilderContext = new WebHostBuilderContext
            {
                Configuration = configuration,
            };

            var mockBuilder = new Mock<ITestApplicationBuilder>();
            mockBuilder
                .Setup(x => x.ConfigureAppConfiguration(It.IsAny<Action<WebHostBuilderContext, IConfigurationBuilder>>()))
                .Callback<Action<WebHostBuilderContext, IConfigurationBuilder>>(i =>
                {
                    i(webHostBuilderContext, configurationBuilder);
                })
                .Returns(() => mockBuilder.Object);
            var builder = mockBuilder.Object;
            act(builder);
            assert(configuration);
        }
    }
}
