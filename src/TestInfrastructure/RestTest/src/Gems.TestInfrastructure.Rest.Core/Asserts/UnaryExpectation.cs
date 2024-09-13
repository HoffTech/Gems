namespace Gems.TestInfrastructure.Rest.Core.Asserts
{
    internal class UnaryExpectation : IExpectation
    {
        private readonly Func<object, bool> compare;

        public UnaryExpectation(Func<object, bool> compare, string description)
        {
            this.compare = compare;
            this.Description = description;
        }

        public string Description { get; set; }

        public bool Satisfies(object fact)
        {
            return this.compare(fact);
        }
    }
}
