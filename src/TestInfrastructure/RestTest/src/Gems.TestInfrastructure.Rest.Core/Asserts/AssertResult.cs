// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

namespace Gems.TestInfrastructure.Rest.Core.Asserts
{
    public class AssertResult
    {
        public bool Success { get; set; }

        public object Fact { get; set; }

        public IExpectation Expected { get; set; }
    }
}
