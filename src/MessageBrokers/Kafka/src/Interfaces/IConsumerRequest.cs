// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

using MediatR;

namespace Gems.MessageBrokers.Kafka.Interfaces
{
    public interface IConsumerRequest<TRequest> : IBaseRequest
    {
        public TRequest Value { get; set; }
    }
}
