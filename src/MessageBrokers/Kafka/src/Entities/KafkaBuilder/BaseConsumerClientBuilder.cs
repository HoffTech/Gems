// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

using System;

using Confluent.Kafka;

using Gems.MessageBrokers.Kafka.AppData.KafkaOptions;

namespace Gems.MessageBrokers.Kafka.Entities.KafkaBuilder
{
    /// <summary>
    /// Class provides creating and configuring consumer.
    /// </summary>
    public class BaseConsumerClientBuilder<TKey, TValue>
    {
        /// <summary>
        /// Kafka configuration.
        /// </summary>
        private readonly KafkaConfiguration kafkaConfiguration;

        /// <summary>
        /// Base configuration for producer and consumer.
        /// </summary>
        private readonly ClientConfig clientConfig;

        /// <summary>
        /// Consumer configuration.
        /// </summary>
        private readonly ConsumerConfig consumerConfig;

        /// <summary>
        /// Topic for produce messages.
        /// </summary>
        private readonly string topic;

        /// <summary>
        /// Initializes a new instance of the <see cref="ProducerClientBuilder"/> class.
        /// </summary>
        /// <param name="kafkaConfiguration">kafka configuration.</param>
        /// <param name="topic">topic name.</param>
        public BaseConsumerClientBuilder(KafkaConfiguration kafkaConfiguration, string topic)
        {
            this.kafkaConfiguration = kafkaConfiguration;
            this.clientConfig = KafkaConfigurationBuilder.BuildClientConfig(kafkaConfiguration);
            this.topic = topic;
            this.consumerConfig = this.CreateBaseConsumer();
        }

        public IConsumer<TKey, TValue> BuildTypeConsumer()
        {
            var consumerBuilder = new ConsumerBuilder<TKey, TValue>(this.consumerConfig);
            this.SetValueDeserializer(consumerBuilder);

            var consumer = consumerBuilder.Build();

            var topicName = this.topic + this.kafkaConfiguration.Consumers[this.topic].TopicPostfix;

            consumer.Subscribe(topicName);

            return consumer;
        }

        protected virtual void SetValueDeserializer(ConsumerBuilder<TKey, TValue> consumer)
        {
        }

        /// <summary>
        /// Initializes consumer configuration.
        /// </summary>
        /// <returns>an object of the <see cref="ConsumerConfig"/> type.</returns>
        protected ConsumerConfig CreateBaseConsumer()
        {
            if (!this.kafkaConfiguration.Consumers.TryGetValue(this.topic, out var consumerSettings))
            {
                throw new ArgumentException(
                    $"Can't create consumer for topic {this.topic} because it not set in configuration.");
            }

            var consumerConfig = new ConsumerConfig(this.clientConfig)
            {
                GroupId = consumerSettings.GroupId,
                AutoOffsetReset = consumerSettings.AutoOffsetReset
            };

            return consumerConfig;
        }
    }
}
