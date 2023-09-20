// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

namespace Gems.Patterns.Outbox
{
    public class OutboxOptions
    {
        /// <summary>
        /// Name in appsettings.json.
        /// </summary>
        public const string Name = "Outbox";

        public string UnitOfWorkKey { get; set; }

        public string WriteOutboxMessagesProcedureName { get; set; }

        public string ReadOutboxMessagesFunctionName { get; set; }

        public string ChangeOutboxMessageProcessedDateProcedureName { get; set; }
    }
}
