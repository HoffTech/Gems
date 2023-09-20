// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

using Microsoft.Extensions.DependencyInjection;

using NUnit.Framework;

namespace Gems.Logging.Security.Tests
{
    internal class SecureKeyJsonHttpTest
    {
        [Test]
        public void TestKeys()
        {
            Assert.DoesNotThrow(() =>
            {
                using var sp = this.CreateProvider();
                var secureKeyProvider = sp.GetRequiredService<ISecureKeyProvider>();
                var keys = secureKeyProvider.Keys();
            });
        }

        private ServiceProvider CreateProvider()
        {
            var services = new ServiceCollection();
            services.AddLoggingFilter(builder =>
            {
                builder.Register(new SecureKeyJsonHttpSource(new System.Uri("http://api-dev.kifr-ru.local/securitykeys/static/log-filter.json")));
            });
            var sp = services.BuildServiceProvider();
            return sp;
        }
    }
}
