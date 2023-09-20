// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

using Confluent.Kafka;

using Gems.MessageBrokers.Kafka.AppData.KafkaOptions;
using Gems.MessageBrokers.Kafka.Entities.KafkaBuilder;
using Gems.MessageBrokers.Kafka.Interfaces;

namespace Gems.MessageBrokers.Kafka.Factory
{
    public class BaseProducerClientFactory<TKey, TValue> : IProducerFactory<TKey, TValue>
    {
        public BaseProducerClientFactory(KafkaConfiguration kafkaConfiguration, string topic)
        {
            this.ProducerBuilder = new ProducerClientBuilder(kafkaConfiguration, topic);
        }

        protected ProducerClientBuilder ProducerBuilder { get; }

        public virtual IProducer<TKey, TValue> BuildProducer()
        {
            return this.ProducerBuilder.BuildBaseTypeProducer<TKey, TValue>();
        }
    }
}
