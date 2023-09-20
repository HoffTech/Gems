// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

using System.Text.RegularExpressions;

namespace Gems.Logging.Security
{
    public class NamePatternPropertyMatcher : IPropertyMatcher
    {
        private readonly Regex namePattern;

        public NamePatternPropertyMatcher(Regex namePattern)
        {
            this.namePattern = namePattern;
        }

        public NamePatternPropertyMatcher(string namePattern)
        {
            this.namePattern = new Regex(namePattern, RegexOptions.IgnoreCase);
        }

        public bool IsMatch(IPropertyProxy proxy)
        {
            return this.namePattern.IsMatch(proxy.Name);
        }
    }
}
