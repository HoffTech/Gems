// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

using System;

namespace Gems.MessageBrokers
{
    [AttributeUsage(AttributeTargets.Class)]
    public class ConsumerListenerPropertyAttribute : Attribute
    {
        public ConsumerListenerPropertyAttribute(string topicName)
        {
            this.TopicName = topicName;
        }

        public ConsumerListenerPropertyAttribute(Type keyType, Type valueType, string topicName)
        {
            this.KeyType = keyType;
            this.ValueType = valueType;
            this.TopicName = topicName;
        }

        public Type KeyType { get; set; } = typeof(string);

        public Type ValueType { get; set; }

        public string TopicName { get; }

        /// <summary>
        /// Необходимо распарсить json значение из строки.
        /// ValueType должно быть установлено в System.String.
        /// </summary>
        public bool NeedParseJsonFromString { get; set; }
    }
}
