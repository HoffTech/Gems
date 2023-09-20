// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

using System;
using System.Text.Json.Serialization;

namespace Gems.DomainEvents
{
    public class DomainNotificationBase<T> : IDomainEventNotification<T> where T : IDomainEvent
    {
        public DomainNotificationBase(T domainEvent)
        {
            this.Id = Guid.NewGuid();
            this.DomainEvent = domainEvent;
        }

        [JsonIgnore]
        public T DomainEvent { get; }

        public Guid Id { get; }

        public virtual IDomainEventNotification<T> Clone(object domainEvent)
        {
            return this;
        }
    }
}
