// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

using System.Collections.Generic;

using Confluent.Kafka;

namespace Gems.MessageBrokers.Kafka.AppData.KafkaOptions
{
    /// <summary>
    /// Configuration for work with kafka.
    /// </summary>
    public class KafkaConfiguration
    {
        /// <summary>
        /// Producers.
        /// </summary>
        public Dictionary<string, ProducerSettings> Producers { get; set; }

        /// <summary>
        /// Consumers.
        /// </summary>
        public Dictionary<string, ConsumerSettings> Consumers { get; set; }

        /// <summary>
        /// Schema registry url.
        /// </summary>
        public string SchemaRegistryUrl { get; set; }

        /// <summary>
        /// Bootstrap servers for send/read messages.
        /// </summary>
        public string BootstrapServers { get; set; }

        /// <summary>
        /// Username for authorize in kafka.
        /// </summary>
        public string SaslUsername { get; set; }

        /// <summary>
        /// Password for authorize in kafka.
        /// </summary>
        public string SaslPassword { get; set; }

        /// <summary>
        /// Security protocol for communicate with brokers.
        /// </summary>
        public SecurityProtocol SecurityProtocol { get; set; }

        /// <summary>
        /// SASL mechanism for authentication.
        /// </summary>
        public SaslMechanism SaslMechanism { get; set; }
    }
}
