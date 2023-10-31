// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

using Confluent.Kafka;
using Confluent.Kafka.SyncOverAsync;
using Confluent.SchemaRegistry.Serdes;

using Gems.MessageBrokers.Kafka.AppData.KafkaOptions;

namespace Gems.MessageBrokers.Kafka.Entities.KafkaBuilder
{
    public class CustomConsumerClientBuilder<TKey, TValue> : BaseConsumerClientBuilder<TKey, TValue> where TValue : class
    {
        public CustomConsumerClientBuilder(KafkaConfiguration kafkaConfiguration, string topic) : base(kafkaConfiguration, topic)
        {
        }

        protected override void SetValueDeserializer(ConsumerBuilder<TKey, TValue> consumerBuilder)
        {
            if (typeof(TValue) == typeof(string))
            {
                return;
            }

            consumerBuilder.SetValueDeserializer(new JsonDeserializer<TValue>().AsSyncOverAsync());
        }
    }
}
