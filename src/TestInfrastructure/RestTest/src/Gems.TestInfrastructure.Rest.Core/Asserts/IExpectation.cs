namespace Gems.TestInfrastructure.Rest.Core.Asserts
{
    public interface IExpectation
    {
        public string Description { get; set; }

        public bool Satisfies(object fact);
    }
}
