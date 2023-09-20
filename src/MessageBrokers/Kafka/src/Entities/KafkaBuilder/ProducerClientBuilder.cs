// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

using System;

using Confluent.Kafka;
using Confluent.SchemaRegistry;
using Confluent.SchemaRegistry.Serdes;

using Gems.MessageBrokers.Kafka.AppData.KafkaOptions;

namespace Gems.MessageBrokers.Kafka.Entities.KafkaBuilder
{
    /// <summary>
    /// Class provides creating and configuring producer.
    /// </summary>
    public class ProducerClientBuilder
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
        /// Producer configuration.
        /// </summary>
        private readonly ProducerConfig producerConfig;

        /// <summary>
        /// Topic for produce messages.
        /// </summary>
        private readonly string topic;

        /// <summary>
        /// Initializes a new instance of the <see cref="ProducerClientBuilder"/> class.
        /// </summary>
        /// <param name="kafkaConfiguration">kafka configuration.</param>
        /// <param name="topic">topic name.</param>
        public ProducerClientBuilder(KafkaConfiguration kafkaConfiguration, string topic)
        {
            this.kafkaConfiguration = kafkaConfiguration;
            this.clientConfig = KafkaConfigurationBuilder.BuildClientConfig(kafkaConfiguration);
            this.topic = topic;
            this.producerConfig = this.CreateBaseProducer();
        }

        /// <summary>
        /// Build producer with message value custom type and <see cref="JsonSerializer{TValue}"/>.
        /// </summary>
        /// <typeparam name="TKey">key type.</typeparam>
        /// <typeparam name="TValue">value type.</typeparam>
        /// <returns>an object of the <see cref="IProducer{TKey, TValue}"/> type.</returns>
        public IProducer<TKey, TValue> BuildCustomTypeProducer<TKey, TValue>() where TValue : class
        {
            var schemaRegistryConfig = new SchemaRegistryConfig
            {
                Url = this.kafkaConfiguration.SchemaRegistryUrl
            };

            var schemaRegistry = new CachedSchemaRegistryClient(schemaRegistryConfig);

            return new ProducerBuilder<TKey, TValue>(this.producerConfig)
                .SetValueSerializer(new JsonSerializer<TValue>(schemaRegistry))
                .Build();
        }

        /// <summary>
        /// Build producer with message value system type.
        /// </summary>
        /// <typeparam name="TKey">key type.</typeparam>
        /// <typeparam name="TValue">value type.</typeparam>
        /// <returns>an object of the <see cref="IProducer{TKey, TValue}"/> type.</returns>
        public IProducer<TKey, TValue> BuildBaseTypeProducer<TKey, TValue>()
        {
            return new ProducerBuilder<TKey, TValue>(this.producerConfig).Build();
        }

        /// <summary>
        /// Initializes producer configuration.
        /// </summary>
        /// <returns>an object of the <see cref="ProducerConfig"/> type.</returns>
        protected ProducerConfig CreateBaseProducer()
        {
            var producerConfig = new ProducerConfig(this.clientConfig);

            if (!this.kafkaConfiguration.Producers.TryGetValue(this.topic, out var producerSettings))
            {
                throw new ArgumentException($"Can't create producer for topic {this.topic} because it not set in configuration.");
            }

            producerConfig.ClientId = producerSettings.ClientId;

            return producerConfig;
        }
    }
}
