// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

namespace Gems.TestInfrastructure.Rest.Core.Library
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
