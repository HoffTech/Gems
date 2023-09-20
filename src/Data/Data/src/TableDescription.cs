// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

using System.Collections.Generic;

namespace Gems.Data
{
    public readonly struct TableDescription
    {
        public TableDescription(string tableName, List<string> fields)
        {
            this.TableName = tableName;
            this.Fields = fields;
        }

        public string TableName { get; }

        public List<string> Fields { get; }
    }
}
