// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

using System;

using Gems.Data.Npgsql;

using NpgsqlTypes;

namespace Gems.Data.Sample.Operations.Persons.GetPersonsLogFile.Entities
{
    [PgType]
    public class Log
    {
        [PgName("log_id")]
        public Guid LogId { get; set; }

        [PgName("updated_by")]
        public string UpdatedBy { get; set; }

        [PgName("updated_at")]
        public DateTime UpdatedAt { get; set; }
    }
}
