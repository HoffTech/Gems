// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

using Confluent.Kafka;

namespace Gems.MessageBrokers.Kafka.AppData.KafkaOptions
{
    /// <summary>
    /// Consumer configuration.
    /// </summary>
    public class ConsumerSettings
    {
        /// <summary>
        /// Subscribers group id.
        /// </summary>
        public string GroupId { get; set; }

        /// <summary>
        /// Auto offset reset.
        /// </summary>
        public AutoOffsetReset? AutoOffsetReset { get; set; }

        /// <summary>
        /// Topic postfix.
        /// </summary>
        public string TopicPostfix { get; set; }
    }
}
