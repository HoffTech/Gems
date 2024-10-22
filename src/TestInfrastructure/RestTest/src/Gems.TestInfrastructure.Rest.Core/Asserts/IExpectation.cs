// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

namespace Gems.TestInfrastructure.Rest.Core.Asserts
{
    public interface IExpectation
    {
        public string Description { get; set; }

        public bool Satisfies(object fact);
    }
}
