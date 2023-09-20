// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

using System;

using Gems.Data.Npgsql;

using NpgsqlTypes;

namespace Gems.Patterns.Outbox
{
    [PgType("outbox.outbox_message_type")]
    public class OutboxMessageDto
    {
        public OutboxMessageDto()
        {
            this.Id = Guid.NewGuid();
        }

        public OutboxMessageDto(DateTime occurredOn, string type, string data)
        {
            this.Id = Guid.NewGuid();
            this.OccurredOn = occurredOn;
            this.Type = type;
            this.Data = data;
        }

        [PgName("id")]
        public Guid Id { get; set; }

        [PgName("type")]
        public string Type { get; set; }

        [PgName("data")]
        public string Data { get; set; }

        [PgName("occurred_on")]
        public DateTime OccurredOn { get; set; }
    }
}
