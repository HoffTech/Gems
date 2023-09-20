// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

using Confluent.Kafka;

using Gems.MessageBrokers.Kafka.AppData.KafkaOptions;

namespace Gems.MessageBrokers.Kafka.Factory
{
    public class CustomProducerClientFactory<TKey, TValue> : BaseProducerClientFactory<TKey, TValue>
        where TValue : class
    {
        public CustomProducerClientFactory(KafkaConfiguration kafkaConfiguration, string topic) : base(kafkaConfiguration, topic)
        {
        }

        public override IProducer<TKey, TValue> BuildProducer()
        {
            return this.ProducerBuilder.BuildCustomTypeProducer<TKey, TValue>();
        }
    }
}
