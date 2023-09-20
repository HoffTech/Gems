// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

using System;

namespace Gems.Logging.Security
{
    public class ObjectPropertyMatcher : IPropertyMatcher
    {
        private readonly Type type;
        private readonly string name;

        public ObjectPropertyMatcher(Type type, string name)
        {
            this.type = type;
            this.name = name;
        }

        public bool IsMatch(IPropertyProxy proxy)
        {
            return proxy.RootType == this.type && proxy.Name == this.name;
        }
    }
}
