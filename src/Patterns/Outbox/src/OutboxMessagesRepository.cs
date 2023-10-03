// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using Gems.Data.UnitOfWork;

using Microsoft.Extensions.Options;

namespace Gems.Patterns.Outbox
{
    public class OutboxMessagesRepository
    {
        private readonly IUnitOfWorkProvider unitOfWorkProvider;
        private readonly IOptions<OutboxOptions> options;

        public OutboxMessagesRepository(IUnitOfWorkProvider unitOfWorkProvider, IOptions<OutboxOptions> options)
        {
            this.unitOfWorkProvider = unitOfWorkProvider;
            this.options = options;
        }

        public Task<List<OutboxMessageDto>> ReadOutboxMessagesAsync(CancellationToken cancellationToken)
        {
            return this.unitOfWorkProvider.GetUnitOfWork(this.options.Value.UnitOfWorkKey, cancellationToken).CallTableFunctionAsync<OutboxMessageDto>(
                this.options.Value.ReadOutboxMessagesFunctionName,
                new Dictionary<string, object>
                {
                    ["p_count"] = 1000
                });
        }

        public Task ChangeOutboxMessageProcessedDateAsync(Guid messageId, CancellationToken cancellationToken)
        {
            return this.unitOfWorkProvider.GetUnitOfWork(this.options.Value.UnitOfWorkKey, cancellationToken).CallStoredProcedureAsync(
                this.options.Value.ChangeOutboxMessageProcessedDateProcedureName,
                new Dictionary<string, object>
                {
                    ["p_message_id"] = messageId
                });
        }

        public Task WriteOutboxMessagesAsync(List<OutboxMessageDto> outboxMessages, CancellationToken cancellationToken)
        {
            return this.unitOfWorkProvider.GetUnitOfWork(this.options.Value.UnitOfWorkKey, cancellationToken).CallStoredProcedureAsync(
                    this.options.Value.WriteOutboxMessagesProcedureName,
                    new Dictionary<string, object>
                    {
                        ["p_messages"] = outboxMessages.ToArray()
                    });
        }
    }
}
