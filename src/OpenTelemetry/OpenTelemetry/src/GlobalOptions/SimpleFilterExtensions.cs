// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

using System.Text.RegularExpressions;

using Gems.OpenTelemetry.Configuration;

using WildcardMatch;

namespace Gems.OpenTelemetry.GlobalOptions
{
    public static class SimpleFilterExtensions
    {
        public static bool RxMatch(this SimpleFilter filter, string text)
        {
            if (filter.Exclude.Any(x => Regex.IsMatch(text, x, RegexOptions.IgnoreCase)))
            {
                return false;
            }

            if (filter.Include.Any(x => Regex.IsMatch(text, x, RegexOptions.IgnoreCase)))
            {
                return true;
            }

            return filter.Include.Count == 0;
        }

        public static bool WildcardMatch(this SimpleFilter filter, string text)
        {
            if (filter.Exclude.Any(x => x.WildcardMatch(text, true)))
            {
                return false;
            }

            if (filter.Include.Any(x => x.WildcardMatch(text, true)))
            {
                return true;
            }

            return filter.Include.Count == 0;
        }

        public static void Assign(this SimpleFilter target, SimpleFilterConfiguration source, List<string> extraExclude = null)
        {
            target.Include.Clear();
            if (source?.Include != null)
            {
                target.Include.AddRange(source.Include);
            }

            target.Exclude.Clear();
            if (source?.Exclude != null)
            {
                target.Exclude.AddRange(source.Exclude);
            }

            if (extraExclude != null)
            {
                target.Exclude.AddRange(extraExclude);
            }
        }
    }
}
