namespace Gems.TestInfrastructure.RestTest.Library
{
    public class Template
    {
        private readonly TestRunnerContext context;

        public Template(TestRunnerContext context, object value)
        {
            this.context = context;
            this.Value = value;
        }

        public object Value { get; set; }

        public object RenderedValue => this.context.Eval(this.Value);

        public object[] ToArray(int size) => Enumerable
            .Range(0, size)
            .Select(_ => this.context.Eval(this.Value))
            .ToArray();
    }
}
