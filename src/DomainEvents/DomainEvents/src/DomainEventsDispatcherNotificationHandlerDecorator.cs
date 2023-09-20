// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

using System.Threading;
using System.Threading.Tasks;

using MediatR;

namespace Gems.DomainEvents
{
    public class DomainEventsDispatcherNotificationHandlerDecorator<T> : INotificationHandler<T> where T : INotification
    {
        private readonly INotificationHandler<T> decorated;
        private readonly IDomainEventsDispatcher domainEventsDispatcher;

        public DomainEventsDispatcherNotificationHandlerDecorator(
            IDomainEventsDispatcher domainEventsDispatcher,
            INotificationHandler<T> decorated)
        {
            this.domainEventsDispatcher = domainEventsDispatcher;
            this.decorated = decorated;
        }

        public async Task Handle(T notification, CancellationToken cancellationToken)
        {
            await this.decorated.Handle(notification, cancellationToken);

            await this.domainEventsDispatcher.DispatchEventsAsync(cancellationToken);
        }
    }
}
