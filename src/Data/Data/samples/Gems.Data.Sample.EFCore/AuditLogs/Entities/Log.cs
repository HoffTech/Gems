using System;

using Gems.Data.Npgsql;

using NpgsqlTypes;

namespace Gems.Data.Sample.EFCore.AuditLogs.Entities
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
