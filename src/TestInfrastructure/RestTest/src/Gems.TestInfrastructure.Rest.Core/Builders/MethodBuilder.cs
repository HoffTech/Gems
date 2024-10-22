// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

namespace Gems.TestInfrastructure.Rest.Core.Builders
{
    public class MethodBuilder
    {
        private static readonly HttpMethod DefaultMethod = HttpMethod.Get;

        private readonly TestRunnerContext context;

        public MethodBuilder(TestRunnerContext context)
        {
            this.context = context;
        }

        public HttpMethod Build(string method)
        {
            if (string.IsNullOrEmpty(method))
            {
                return DefaultMethod;
            }

            var o = this.context.Eval(method);
            if (o == null)
            {
                return DefaultMethod;
            }

            if (o is HttpMethod m)
            {
                return m;
            }

            if (o is string s)
            {
                if (s == string.Empty)
                {
                    return DefaultMethod;
                }

                return HttpMethod.Parse(s);
            }

            throw new ArgumentOutOfRangeException();
        }
    }
}
