// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

namespace Gems.TestInfrastructure.Rest.Core.Builders
{
    public class AddressBuilder
    {
        private readonly TestRunnerContext context;

        public AddressBuilder(TestRunnerContext context)
        {
            this.context = context;
        }

        public Uri Build(string uri)
        {
            if (uri == null)
            {
                throw new ArgumentNullException(nameof(uri));
            }

            var o = this.context.Eval(uri);
            if (o == null)
            {
                throw new ArgumentNullException(nameof(uri));
            }

            if (o is Uri uriObject)
            {
                return uriObject;
            }

            if (o is string s)
            {
                if (s == string.Empty)
                {
                    throw new ArgumentNullException(nameof(uri));
                }

                return new Uri(s);
            }

            throw new ArgumentOutOfRangeException();
        }
    }
}
