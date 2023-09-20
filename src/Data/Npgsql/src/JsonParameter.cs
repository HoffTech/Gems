// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

using System.Data;

using Dapper;

using Npgsql;

using NpgsqlTypes;

namespace Gems.Data.Npgsql
{
    public class JsonParameter : SqlMapper.ICustomQueryParameter
    {
        private readonly string value;

        public JsonParameter(string value)
        {
            this.value = value;
        }

        public void AddParameter(IDbCommand command, string name)
        {
            command.Parameters.Add(new NpgsqlParameter(name, NpgsqlDbType.Jsonb) { Value = this.value });
        }
    }
}
