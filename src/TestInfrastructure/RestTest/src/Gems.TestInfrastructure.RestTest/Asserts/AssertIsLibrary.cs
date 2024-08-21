using System.Collections;

namespace Gems.TestInfrastructure.RestTest.Asserts
{
    public class AssertIsLibrary
    {
        public IExpectation Null()
        {
            return new UnaryExpectation(v => v == null, "null");
        }

        public IExpectation NotNull()
        {
            return new UnaryExpectation(v => v != null, "not null");
        }

        public IExpectation Empty()
        {
            return new UnaryExpectation(
                v =>
                {
                    if (v == null)
                    {
                        return true;
                    }

                    if (v is string stringValue)
                    {
                        return stringValue == string.Empty;
                    }

                    if (v is IList listValue)
                    {
                        return listValue.Count == 0;
                    }

                    if (v is ICollection collectionValue)
                    {
                        return collectionValue.Count == 0;
                    }

                    return false;
                },
                "empty");
        }

        public IExpectation NotEmpty()
        {
            return new InverceExpectation(this.Empty(), "not empty");
        }

        public IExpectation EqualTo(object expected)
        {
            return new BinaryExpectation(expected, (e, f) => e == f, expected?.ToString() ?? "null");
        }

        public IExpectation NotEqualTo(object expected)
        {
            return new BinaryExpectation(expected, (e, f) => e != f, "not " + (expected?.ToString() ?? "null"));
        }
    }
}
