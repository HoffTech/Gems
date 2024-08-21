﻿namespace Gems.TestInfrastructure.RestTest.Asserts
{
    public class AssertResult
    {
        public bool Success { get; set; }

        public object Fact { get; set; }

        public IExpectation Expected { get; set; }
    }
}
