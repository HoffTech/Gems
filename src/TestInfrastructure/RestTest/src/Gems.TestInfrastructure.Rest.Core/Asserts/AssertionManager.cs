namespace Gems.TestInfrastructure.Rest.Core.Asserts
{
    public class AssertionManager
    {
        private readonly TestRunnerContext context;

        public AssertionManager(TestRunnerContext context)
        {
            this.context = context;
        }

        public void Assert(object expectation)
        {
            if (expectation is string stringExpression)
            {
                this.Assert(stringExpression);
            }
        }

        public void Assert(string expression)
        {
            try
            {
                var result = this.context.EvalExpression(expression);
                if (result is AssertResult assertResult)
                {
                    if (!assertResult.Success)
                    {
                        throw new AssertException(assertResult.Fact, assertResult.Expected);
                    }
                }
                else if (!(result is bool boolResult && boolResult))
                {
                    throw new AssertException(result, true);
                }
            }
            catch (AssertException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new AssertException("Unexpected exception", ex);
            }
        }
    }
}
