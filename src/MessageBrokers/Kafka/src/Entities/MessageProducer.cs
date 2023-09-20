// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Confluent.Kafka;

using Gems.MessageBrokers.Kafka.AppData.KafkaOptions;
using Gems.MessageBrokers.Kafka.Factory;
using Gems.MessageBrokers.Kafka.Interfaces;
using Gems.Tasks;

using Microsoft.Extensions.Options;

namespace Gems.MessageBrokers.Kafka.Entities
{
    public class MessageProducer : IMessageProducer
    {
        private readonly ConcurrentDictionary<(string, Type, Type), IClient> producerDictionary = new ConcurrentDictionary<(string, Type, Type), IClient>();

        private readonly KafkaConfiguration kafkaConfiguration;

        public MessageProducer(IOptions<KafkaConfiguration> kafkaConfiguration)
        {
            this.kafkaConfiguration = kafkaConfiguration.Value;
        }

        public Task ProduceAsync<TKey, TValue>(string topic, TKey key, TValue message)
        {
            return AsyncAwaiter.AwaitAsync(topic, () =>
            {
                var clientId = this.kafkaConfiguration.Producers.FirstOrDefault(z => z.Key == topic).Value.ClientId;

                var producer = this.GetInstance<TKey, TValue>(clientId, topic, typeof(TKey), typeof(TValue));

                var newMessage = new Message<TKey, TValue> { Key = key, Value = message };

                var topicName = topic + this.kafkaConfiguration.Producers.FirstOrDefault(z => z.Key == topic).Value.TopicPostfix;

                return ((IProducer<TKey, TValue>)producer)?.ProduceAsync(topicName, newMessage);
            });
        }

        public void Dispose()
        {
            foreach (var (_, value) in this.producerDictionary)
            {
                var mInfo = value.GetType().GetMethod("Flush", new[] { typeof(CancellationToken) });

                object[] parametersArray = { CancellationToken.None };

                mInfo?.Invoke(value, parametersArray);
            }
        }

        protected IClient GetInstance<TKey, TValue>(string clientId, string topic, Type typeKey, Type typeValue)
        {
            if (this.producerDictionary.TryGetValue((clientId, typeKey, typeValue), out var producer))
            {
                return producer;
            }

            var factoryType = typeValue.Namespace == nameof(System) ? typeof(BaseProducerClientFactory<,>) : typeof(CustomProducerClientFactory<,>);
            var typeArgs = new[] { typeKey, typeValue };
            var constructed = factoryType.MakeGenericType(typeArgs);

            var factory = Activator.CreateInstance(constructed, this.kafkaConfiguration, topic) as IProducerFactory<TKey, TValue>;

            producer = factory?.BuildProducer();

            if (producer == null)
            {
                return null;
            }

            this.producerDictionary.TryAdd((clientId, typeKey, typeValue), producer);

            return producer;
        }
    }
}
