// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

using System.Xml.Linq;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

using Newtonsoft.Json.Linq;

using NUnit.Framework;

using Serilog;
using Serilog.Formatting.Compact;

namespace Gems.Logging.Security.Tests
{
    internal class PropertyFilterTest
    {
        [OneTimeSetUp]
        public void Setup()
        {
            Log.Logger = new LoggerConfiguration()
                .WriteTo.Console(new CompactJsonFormatter())
                .CreateLogger();
        }

        [Test]
        public void FilterJson_NoAction()
        {
            using var sp = this.CreateProvider();
            var filter = sp.GetRequiredService<IPropertyFilter<JToken>>();
            var result = filter.FilterJson(@"{ ""dummy"": ""123"" }");
            Assert.That(result, Is.EqualTo("{\r\n  \"dummy\": \"123\"\r\n}"));
        }

        [Test]
        public void FilterJson_DeleteProperty()
        {
            using var sp = this.CreateProvider();
            var filter = sp.GetRequiredService<IPropertyFilter<JToken>>();
            var result = filter.FilterJson(@"{ ""delete_property"": ""secretValue"" }");
            Assert.That(result, Is.EqualTo("{}"));
        }

        [Test]
        public void FilterJson_DeleteArrayElementProperty()
        {
            using var sp = this.CreateProvider();
            var filter = sp.GetRequiredService<IPropertyFilter<JToken>>();
            var result = filter.FilterJson(@"{ ""obj"": [{ ""delete_property"": ""secretValue"" }]}");
            Assert.That(result, Is.EqualTo("{\r\n  \"obj\": [\r\n    {}\r\n  ]\r\n}"));
        }

        [Test]
        public void FilterJson_DeleteArrayValue()
        {
            using var sp = this.CreateProvider();
            var filter = sp.GetRequiredService<IPropertyFilter<JToken>>();
            var result = filter.FilterJson(@"{ ""delete_property"": [""secretValue""] }");
            Assert.That(result, Is.EqualTo("{\r\n  \"delete_property\": []\r\n}"));
        }

        [Test]
        public void FilterJson_FullMask()
        {
            using var sp = this.CreateProvider();
            var filter = sp.GetRequiredService<IPropertyFilter<JToken>>();
            var result = filter.FilterJson(@"{ ""username"": ""Jon Doe"" }");
            Assert.That(result, Is.EqualTo("{\r\n  \"username\": \"******\"\r\n}"));
        }

        [Test]
        public void FilterJson_PartialMask()
        {
            using var sp = this.CreateProvider();
            var filter = sp.GetRequiredService<IPropertyFilter<JToken>>();
            var result = filter.FilterJson(@"{ ""phone"": ""9165550206"" }");
            Assert.That(result, Is.EqualTo("{\r\n  \"phone\": \"******0206\"\r\n}"));
        }

        [Test]
        public void FilterXml_NoAction()
        {
            using var sp = this.CreateProvider();
            var filter = sp.GetRequiredService<IPropertyFilter<XElement>>();
            var result = filter.FilterXml("<root><dummy>123</dummy></root>");
            Assert.That(result, Is.EqualTo("<root>\r\n  <dummy>123</dummy>\r\n</root>"));
        }

        [Test]
        public void FilterXml_DeleteProperty()
        {
            using var sp = this.CreateProvider();
            var filter = sp.GetRequiredService<IPropertyFilter<XElement>>();
            var result = filter.FilterXml("<root><delete_property>123</delete_property></root>");
            Assert.That(result, Is.EqualTo("<root />"));
        }

        [Test]
        public void FilterXml_FullMask()
        {
            using var sp = this.CreateProvider();
            var filter = sp.GetRequiredService<IPropertyFilter<XElement>>();
            var result = filter.FilterXml("<root><username>Jon Doe</username></root>");
            Assert.That(result, Is.EqualTo("<root>\r\n  <username>******</username>\r\n</root>"));
        }

        [Test]
        public void FilterXml_PartialMask()
        {
            using var sp = this.CreateProvider();
            var filter = sp.GetRequiredService<IPropertyFilter<XElement>>();
            var result = filter.FilterXml("<root><phone>9165550206</phone></root>");
            Assert.That(result, Is.EqualTo("<root>\r\n  <phone>******0206</phone>\r\n</root>"));
        }

        [Test]
        public void FilterObject_PartialMask()
        {
            using var sp = this.CreateProvider();
            var logger = sp.GetRequiredService<ILogger<PropertyFilterTest>>();
            var filter = sp.GetRequiredService<IPropertyFilter<ObjectToJsonProjection>>();
            var result = filter.FilterObject(new
            {
                Phone = "9165550206"
            });
            logger.LogInformation("{@result}", result);
            Assert.That(result.Phone, Is.EqualTo("******0206"));
        }

        [Test]
        public void FilterObject_DeleteProperty()
        {
            using var sp = this.CreateProvider();
            var logger = sp.GetRequiredService<ILogger<PropertyFilterTest>>();
            var filter = sp.GetRequiredService<IPropertyFilter<ObjectToJsonProjection>>();
            var result = filter.FilterObject(new
            {
                Delete_Property = 123
            });
            logger.LogInformation("{@result}", result);
            Assert.That(result.Delete_Property, Is.EqualTo(default(int)));
        }

        [Test]
        public void FilterObject_DeleteArrayElementProperty()
        {
            using var sp = this.CreateProvider();
            var logger = sp.GetRequiredService<ILogger<PropertyFilterTest>>();
            var filter = sp.GetRequiredService<IPropertyFilter<ObjectToJsonProjection>>();
            var result = filter.FilterObject(new
            {
                Arr = new[]
                {
                    new
                    {
                        Delete_Property = (int?)123
                    }
                }
            });
            logger.LogInformation("{@result}", result);
            Assert.That(result.Arr[0].Delete_Property, Is.EqualTo(default(int?)));
        }

        [Test]
        public void FilterObject_FilterAttribute()
        {
            using var sp = this.CreateProvider();
            var logger = sp.GetRequiredService<ILogger<PropertyFilterTest>>();
            var filter = sp.GetRequiredService<IPropertyFilter<ObjectToJsonProjection>>();
            var result = filter.FilterObject(new SimpleObject
            {
                Dummy = "DummyValue",
                HideMe = "HideMeValue",
                SubObject = new SimpleObject.SimpleObject2
                {
                    Dummy2 = "Dummy2Value",
                    HideMe2 = "9165550206"
                }
            });
            logger.LogInformation("{@result}", result);
            Assert.That(result.HideMe, Is.EqualTo(default(string)));
            Assert.That(result.SubObject.HideMe2, Is.EqualTo("******0206"));
        }

        private ServiceProvider CreateProvider()
        {
            var services = new ServiceCollection();
            services.AddLogging(config => config.AddSerilog());
            services.AddLoggingFilter(builder =>
            {
                builder.Register(new SecureKeyCsvFileSource("securekeys.txt"));
            });
            var sp = services.BuildServiceProvider();
            return sp;
        }
    }

    internal class SimpleObject
    {
        [Filter]
        public string HideMe { get; set; }

        public string Dummy { get; set; }

        public SimpleObject2 SubObject { get; set; }

        public class SimpleObject2
        {
            [Filter(".(?=.{4})", "*")]
            public string HideMe2 { get; set; }

            public string Dummy2 { get; set; }
        }
    }
}
