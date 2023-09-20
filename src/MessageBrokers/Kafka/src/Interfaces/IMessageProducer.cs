// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

using System;
using System.Threading.Tasks;

namespace Gems.MessageBrokers.Kafka.Interfaces
{
    /// <summary>
    /// Интерфейс для отправки сообщения.
    /// </summary>
    public interface IMessageProducer : IDisposable
    {
        /// <summary>
        /// Отправка сообщения с ожиданием задачи.
        /// </summary>
        /// <param name="topic">message topic.</param>
        /// <param name="key">message key.</param>
        /// <param name="message">message object.</param>
        /// <returns>Task result.</returns>
        public Task ProduceAsync<TKey, TValue>(string topic, TKey key, TValue message);
    }
}
