// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Gems.Settings.Gitlab
{
    internal static class GitlabConfigurationParser
    {
        public static Dictionary<string, string> Parse(Dictionary<string, string> gitlabVariables, string prefix)
        {
            return gitlabVariables.ToDictionary(
                x => ParseKey(x.Key, prefix),
                x => ParseValue(x.Value));
        }

        private static string ParseKey(string key, string prefix)
        {
            return (string.IsNullOrEmpty(prefix) ? key : key.Substring(prefix.Length)).Replace("__", ":");
        }

        private static string ParseValue(string value)
        {
            var sb = new StringBuilder();
            var mode = 0;
            foreach (var c in value)
            {
                switch (mode)
                {
                    case 0:
                        if (c == '\\')
                        {
                            mode = 1;
                        }
                        else
                        {
                            sb.Append(c);
                        }

                        break;
                    case 1:
                        sb.Append(c switch
                        {
                            'n' => '\n',
                            'r' => '\r',
                            _ => c,
                        });
                        mode = 0;
                        break;
                }
            }

            if (mode == 1)
            {
                sb.Append('\\');
            }

            return sb.ToString();
        }
    }
}
