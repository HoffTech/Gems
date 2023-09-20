// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

using Microsoft.Extensions.DependencyInjection;

using NUnit.Framework;

namespace Gems.Logging.Security.Tests
{
    internal class SecureKeyCsvFileTest
    {
        [Test]
        public void TestKeys()
        {
            using var sp = this.CreateProvider();
            var secureKeyProvider = sp.GetRequiredService<ISecureKeyProvider>();
            var keys = secureKeyProvider.Keys();

            Assert.That(keys.Count, Is.EqualTo(3));
            Assert.That(keys[0].Action, Is.EqualTo(SecureKeyAction.Remove));

            Assert.That(keys[1].Action, Is.EqualTo(SecureKeyAction.Update));
            Assert.That(keys[1].ReplaceText, Is.EqualTo("***"));

            Assert.That(keys[2].Action, Is.EqualTo(SecureKeyAction.Update));
            Assert.That(keys[2].ReplaceText, Is.EqualTo("*"));
        }

        private ServiceProvider CreateProvider()
        {
            var services = new ServiceCollection();
            services.AddLoggingFilter(builder =>
            {
                builder.Register(new SecureKeyCsvFileSource("securekeys.txt"));
            });
            var sp = services.BuildServiceProvider();
            return sp;
        }
    }
}
