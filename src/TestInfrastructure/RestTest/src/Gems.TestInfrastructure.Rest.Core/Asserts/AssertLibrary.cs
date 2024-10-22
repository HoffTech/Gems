// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

namespace Gems.TestInfrastructure.Rest.Core.Asserts
{
    public class AssertLibrary
    {
        private readonly AssertIsLibrary isObject = new AssertIsLibrary();

        public AssertResult Null(object value)
        {
            return this.That(value, this.isObject.Null());
        }

        public AssertResult NotNull(object value)
        {
            return this.That(value, this.isObject.NotNull());
        }

        public AssertResult Empty(object value)
        {
            return this.That(value, this.isObject.Empty());
        }

        public AssertResult NotEmpty(object value)
        {
            return this.That(value, this.isObject.NotEmpty());
        }

        public AssertResult EqualTo(object value, object expected)
        {
            return this.That(value, this.isObject.EqualTo(expected));
        }

        public AssertResult NotEqualTo(object value, object expected)
        {
            return this.That(value, this.isObject.NotEqualTo(expected));
        }

        public AssertResult That(object value, IExpectation expectation)
        {
            var result = new AssertResult();
            result.Fact = value;
            result.Expected = expectation;
            result.Success = expectation.Satisfies(value);
            return result;
        }
    }
}
