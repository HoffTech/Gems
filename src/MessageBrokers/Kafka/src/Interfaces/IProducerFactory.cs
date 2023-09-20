// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

using Confluent.Kafka;

namespace Gems.MessageBrokers.Kafka.Interfaces
{
    public interface IProducerFactory<TKey, TValue>
    {
        public IProducer<TKey, TValue> BuildProducer();
    }
}
