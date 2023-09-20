// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Gems.DomainEvents;

using MediatR;

using Newtonsoft.Json;

namespace Gems.Patterns.Outbox
{
    public class DomainEventsDispatcher : IDomainEventsDispatcher
    {
        private readonly IMediator mediator;
        private readonly EventsRepository eventsRepository;
        private readonly OutboxMessagesRepository outboxMessagesRepository;
        private readonly IEnumerable<IDomainEventNotification> domainEventNotificationPrototypes;

        public DomainEventsDispatcher(
            IMediator mediator,
            EventsRepository eventsRepository,
            OutboxMessagesRepository outboxMessagesRepository,
            IEnumerable<IDomainEventNotification> domainEventNotificationPrototypes)
        {
            this.mediator = mediator;
            this.eventsRepository = eventsRepository;
            this.outboxMessagesRepository = outboxMessagesRepository;
            this.domainEventNotificationPrototypes = domainEventNotificationPrototypes;
        }

        public async Task DispatchEventsAsync(CancellationToken cancellationToken)
        {
            var domainEvents = this.eventsRepository.GetEvents(cancellationToken);

            var domainEventNotifications = new List<IDomainEventNotification<IDomainEvent>>();
            foreach (var domainEvent in domainEvents)
            {
                var domainEvenNotificationType = typeof(IDomainEventNotification<>);
                var domainNotificationWithGenericType = domainEvenNotificationType.MakeGenericType(domainEvent.GetType());
                var domainNotificationPrototype = this.domainEventNotificationPrototypes
                    .FirstOrDefault(x => x.GetType().GetInterfaces().Any(y => y == domainNotificationWithGenericType));

                if (domainNotificationPrototype != null)
                {
                    domainEventNotifications.Add(((IDomainEventNotification<IDomainEvent>)domainNotificationPrototype).Clone(domainEvent));
                }
            }

            this.eventsRepository.ClearEvents(cancellationToken);

            var tasks = domainEvents
                .Select(async (domainEvent) => await this.mediator.Publish(domainEvent));

            await Task.WhenAll(tasks);
            var outboxMessages = new List<OutboxMessageDto>();
            foreach (var domainEventNotification in domainEventNotifications)
            {
                var type = domainEventNotification.GetType().FullName;
                var data = JsonConvert.SerializeObject(domainEventNotification);
                var outboxMessage = new OutboxMessageDto(
                    domainEventNotification.DomainEvent.OccurredOn,
                    type,
                    data);
                outboxMessages.Add(outboxMessage);
            }

            await this.outboxMessagesRepository.WriteOutboxMessagesAsync(outboxMessages, cancellationToken)
                .ConfigureAwait(false);
        }
    }
}
