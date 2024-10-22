// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

namespace Gems.TestInfrastructure.Rest.Core.Asserts
{
    internal class BinaryExpectation : IExpectation
    {
        private readonly Func<object, object, bool> compare;
        private readonly object expected;

        public BinaryExpectation(object expected, Func<object, object, bool> compare, string description)
        {
            this.compare = compare;
            this.expected = expected;
            this.Description = description;
        }

        public string Description { get; set; }

        public bool Satisfies(object fact)
        {
            return this.compare(this.expected, fact);
        }
    }
}
