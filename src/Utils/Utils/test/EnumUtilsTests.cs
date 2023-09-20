// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

using System;

using NUnit.Framework;

namespace Gems.Utils.Tests
{
    public class Tests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        [Obsolete("Use MetricAttribute")]
        public void CheckToDescription()
        {
            Assert.AreEqual(TestEnum.Value1.ToDescription(), "value-1");
        }
    }
}
