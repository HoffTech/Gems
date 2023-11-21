// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

namespace Gems.OpenTelemetry.GlobalOptions
{
    public class SimpleFilter
    {
        public List<string> Include { get; set; } = new List<string>();

        public List<string> Exclude { get; set; } = new List<string>();
    }
}
