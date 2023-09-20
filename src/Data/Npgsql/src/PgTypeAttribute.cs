// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

using System;

namespace Gems.Data.Npgsql
{
    [AttributeUsage(AttributeTargets.Class)]
    public class PgTypeAttribute : Attribute
    {
        public PgTypeAttribute()
        {
        }

        public PgTypeAttribute(string typeName)
        {
            this.TypeName = typeName;
        }

        public string TypeName { get; }
    }
}
