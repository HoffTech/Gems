// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

using Gems.TestInfrastructure.Rest.Core.Model;

namespace Gems.TestInfrastructure.Rest.Core.Builders
{
    public class HttpRequestDefinitionBuilder
    {
        private readonly TestRunnerContext context;
        private readonly MethodBuilder methodBuilder;
        private readonly AddressBuilder addresBuilder;

        public HttpRequestDefinitionBuilder(TestRunnerContext context)
        {
            this.context = context;
            this.addresBuilder = new AddressBuilder(context);
            this.methodBuilder = new MethodBuilder(context);
        }

        public HttpRequestDefinition Build(TestRequest request)
        {
            var result = new HttpRequestDefinition();
            result.Address = this.addresBuilder.Build(request.Url);
            result.Method = this.methodBuilder.Build(request.Method);
            result.Timeout = this.context.ParseTimeSpan(request.Timeout);
            result.Headers = request.Headers?
                .Select(x => new
                {
                    x.Key,
                    Value = this.context.Eval(x.Value)?.ToString(),
                })
                .ToDictionary(x => x.Key, x => x.Value);
            result.Body = this.context.Eval(request.Body);
            return result;
        }
    }
}
