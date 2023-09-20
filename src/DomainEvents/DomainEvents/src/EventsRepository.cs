// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Gems.DomainEvents
{
    public class EventsRepository
    {
        private readonly ConcurrentDictionary<CancellationToken, ConcurrentDictionary<IDomainEvent, bool>> domainEventsMap;

        public EventsRepository()
        {
            this.domainEventsMap = new ConcurrentDictionary<CancellationToken, ConcurrentDictionary<IDomainEvent, bool>>();
        }

        public List<IDomainEvent> GetEvents(CancellationToken cancellationToken)
        {
            var domainEvents = this.domainEventsMap.GetOrAdd(
                cancellationToken,
                _ => new ConcurrentDictionary<IDomainEvent, bool>());

            return domainEvents.Select(x => x.Key).ToList();
        }

        public void ClearEvents(CancellationToken cancellationToken)
        {
            var domainEvents = this.domainEventsMap.GetOrAdd(
                cancellationToken,
                _ => new ConcurrentDictionary<IDomainEvent, bool>());
            domainEvents.Clear();
        }

        public void AddEvent(IDomainEvent domainEvent, CancellationToken cancellationToken)
        {
            var domainEvents = this.domainEventsMap.GetOrAdd(
                cancellationToken,
                _ => new ConcurrentDictionary<IDomainEvent, bool>());
            domainEvents.GetOrAdd(domainEvent, _ => default);
        }
    }
}
