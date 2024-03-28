using System;

using Gems.Data.Npgsql;

using NpgsqlTypes;

namespace Gems.Data.Sample.Context.Persons.UpdatePerson.Entities
{
    [PgType("public.session_type")]
    public class Session
    {
        [PgName("session_id")]
        public Guid SessionId { get; set; }

        [PgName("qty")]
        public int Quantity { get; set; }

        [PgName("submitted_at")]
        public DateTime SubmittedAt { get; set; }
    }
}
