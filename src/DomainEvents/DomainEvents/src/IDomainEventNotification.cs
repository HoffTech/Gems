// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

using System;

using MediatR;

namespace Gems.DomainEvents
{
    public interface IDomainEventNotification<out TEventType> : IDomainEventNotification
    {
        TEventType DomainEvent { get; }

        IDomainEventNotification<TEventType> Clone(object domainEvent);
    }

    public interface IDomainEventNotification : INotification
    {
        Guid Id { get; }
    }
}
