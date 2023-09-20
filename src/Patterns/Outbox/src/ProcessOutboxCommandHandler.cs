// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

using Dapper;

using Gems.DomainEvents;
using Gems.Jobs;

using MediatR;

using Newtonsoft.Json;

namespace Gems.Patterns.Outbox
{
    [JobHandler("ProcessOutbox")]
    public class ProcessOutboxCommandHandler : IRequestHandler<ProcessOutboxCommand>
    {
        private readonly IMediator mediator;
        private readonly OutboxMessagesRepository outboxMessagesRepository;

        public ProcessOutboxCommandHandler(IMediator mediator, OutboxMessagesRepository outboxMessagesRepository)
        {
            this.mediator = mediator;
            this.outboxMessagesRepository = outboxMessagesRepository;
        }

        public static Assembly Assembly { get; set; }

        public async Task Handle(ProcessOutboxCommand command, CancellationToken cancellationToken)
        {
            var messages = await this.outboxMessagesRepository.ReadOutboxMessagesAsync(cancellationToken).ConfigureAwait(false);
            var messagesList = messages.AsList();
            if (messagesList.Count > 0)
            {
                foreach (var message in messagesList)
                {
                    var type = Assembly.GetType(message.Type);
                    var request = JsonConvert.DeserializeObject(message.Data, type) as IDomainEventNotification;
                    await this.mediator.Publish(request, cancellationToken);

                    await this.outboxMessagesRepository.ChangeOutboxMessageProcessedDateAsync(message.Id, cancellationToken).ConfigureAwait(false);
                }
            }
        }
    }
}
