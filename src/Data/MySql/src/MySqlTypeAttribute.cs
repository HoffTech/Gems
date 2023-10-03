// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

using System;

namespace Gems.Data.MySql
{
    [AttributeUsage(AttributeTargets.Class)]
    public class MySqlTypeAttribute : Attribute
    {
        public MySqlTypeAttribute()
        {
        }

        public MySqlTypeAttribute(string typeName)
        {
            this.TypeName = typeName;
        }

        public string TypeName { get; }
    }
}
