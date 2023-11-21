// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

using System.Collections.Generic;
using System.Linq;

namespace Gems.OpenTelemetry.Configuration
{
    public class SimpleFilterConfiguration
    {
        public List<string> Include { get; set; }

        public List<string> Exclude { get; set; }

        public string IncludeString
        {
            get => this.Include == null ? string.Empty : string.Join(";", this.Include);
            set => this.Include = value?.Split(';').Select(x => x.Trim()).ToList();
        }

        public string ExcludeString
        {
            get => this.Exclude == null ? string.Empty : string.Join(";", this.Exclude);
            set => this.Exclude = value?.Split(';').Select(x => x.Trim()).ToList();
        }
    }
}
