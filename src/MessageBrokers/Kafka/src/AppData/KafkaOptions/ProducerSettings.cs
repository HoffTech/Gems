// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

namespace Gems.MessageBrokers.Kafka.AppData.KafkaOptions
{
    /// <summary>
    /// Producer configuration.
    /// </summary>
    public class ProducerSettings
    {
        /// <summary>
        /// Cliend identification.
        /// </summary>
        public string ClientId { get; set; }

        /// <summary>
        /// Topic postfix.
        /// </summary>
        public string TopicPostfix { get; set; }
    }
}
