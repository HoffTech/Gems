// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

using Confluent.Kafka;

using Gems.MessageBrokers.Kafka.AppData.KafkaOptions;

namespace Gems.MessageBrokers.Kafka.Entities.KafkaBuilder
{
    /// <summary>
    /// Provides method for create and configure base configuration for Kafka.
    /// </summary>
    public class KafkaConfigurationBuilder
    {
        /// <summary>
        /// Create base client configuration.
        /// </summary>
        /// <param name="kafkaConfiguration">kafka configuration.</param>
        /// <returns>an object of the <see cref="ClientConfig"/> type.</returns>
        public static ClientConfig BuildClientConfig(KafkaConfiguration kafkaConfiguration)
        {
            var clientConfig = new ClientConfig
            {
                SaslMechanism = kafkaConfiguration.SaslMechanism,
                SecurityProtocol = kafkaConfiguration.SecurityProtocol,
                BootstrapServers = kafkaConfiguration.BootstrapServers
            };

            if (clientConfig.SecurityProtocol == SecurityProtocol.SaslPlaintext)
            {
                clientConfig.SaslUsername = kafkaConfiguration.SaslUsername;
                clientConfig.SaslPassword = kafkaConfiguration.SaslPassword;
            }

            return clientConfig;
        }
    }
}
