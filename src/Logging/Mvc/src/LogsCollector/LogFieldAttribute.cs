// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

using System;

namespace Gems.Logging.Mvc.LogsCollector
{
    public class LogFieldAttribute : Attribute
    {
        public LogFieldAttribute(string name)
        {
            this.Name = name;
        }

        public LogFieldAttribute()
        {
        }

        public string Name { get; }
    }
}
