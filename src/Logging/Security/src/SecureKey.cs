// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

using System.Text.RegularExpressions;

namespace Gems.Logging.Security
{
    public class SecureKey
    {
        public SecureKey(
            Regex namePattern,
            SecureKeyAction action,
            Regex valuePattern = default,
            string replaceText = default)
        {
            this.Matcher = new NamePatternPropertyMatcher(namePattern);
            this.Action = action;
            this.ValuePattern = valuePattern;
            this.ReplaceText = replaceText;
        }

        public SecureKey(
            string namePattern,
            SecureKeyAction action,
            Regex valuePattern = default,
            string replaceText = default)
        {
            this.Matcher = new NamePatternPropertyMatcher(namePattern);
            this.Action = action;
            this.ValuePattern = valuePattern;
            this.ReplaceText = replaceText;
        }

        public SecureKey(
            IPropertyMatcher matcher,
            SecureKeyAction action,
            Regex valuePattern = default,
            string replaceText = default)
        {
            this.Matcher = matcher;
            this.Action = action;
            this.ValuePattern = valuePattern;
            this.ReplaceText = replaceText;
        }

        public IPropertyMatcher Matcher { get; }

        public SecureKeyAction Action { get; }

        public Regex ValuePattern { get; }

        public string ReplaceText { get; }
    }
}
