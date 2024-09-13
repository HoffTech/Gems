namespace Gems.TestInfrastructure.Rest.Core.Asserts
{
    internal class InverceExpectation : IExpectation
    {
        private readonly IExpectation expectation;

        public InverceExpectation(IExpectation expectation, string description)
        {
            this.expectation = expectation;
            this.Description = description;
        }

        public string Description { get; set; }

        public bool Satisfies(object fact)
        {
            return !this.expectation.Satisfies(fact);
        }
    }
}
