// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

using System.Collections;

namespace Gems.TestInfrastructure.Rest.Core.Asserts
{
    public class AssertException : Exception
    {
        private readonly object fact;
        private readonly object expected;

        public AssertException(object fact, object expected, Exception innerException)
            : base(FormatErrorMessage(fact, expected), innerException)
        {
            this.fact = fact;
            this.expected = expected;
        }

        public AssertException(object fact, object expected)
            : base(FormatErrorMessage(fact, expected))
        {
            this.fact = fact;
            this.expected = expected;
        }

        public AssertException(string message) : base(message)
        {
        }

        public AssertException(string message, Exception innerException) : base(message, innerException)
        {
        }

        public AssertException()
        {
        }

        public object Fact => this.fact;

        public object Expected => this.expected;

        private static string FormatErrorMessage(object fact, object expected)
        {
            return $"Expected {FormatValue(expected)} but found {FormatValue(fact)}";
        }

        private static string FormatValue(object value)
        {
            if (value == null)
            {
                return "null";
            }

            if (value is string stringValue && stringValue == string.Empty)
            {
                return "empty string";
            }

            if (value is IExpectation expectation)
            {
                return expectation.Description;
            }

            if (value is IEnumerable list)
            {
                return $"[{string.Join(", ", FormatList(list))}]";
            }

            return $"\"{value}\"";
        }

        private static IEnumerable<string> FormatList(IEnumerable list)
        {
            foreach (var l in list)
            {
                yield return FormatValue(l);
            }
        }
    }
}
