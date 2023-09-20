// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

using System;

namespace Gems.Logging.Security
{
    public class FilterAttribute : Attribute
    {
        public FilterAttribute()
        {
            this.Search = string.Empty;
            this.Replace = string.Empty;
        }

        public FilterAttribute(string search, string replace)
        {
            this.Search = search;
            this.Replace = replace;
        }

        public string Search { get; set; }

        public string Replace { get; set; }
    }
}
