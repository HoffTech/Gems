// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

using System;

namespace Gems.MessageBrokers
{
    [AttributeUsage(AttributeTargets.Class)]
    public class ConsumerListenerPropertyAttribute : Attribute
    {
        public ConsumerListenerPropertyAttribute(Type keyType, Type valueType, string topicName)
        {
            this.KeyType = keyType;
            this.ValueType = valueType;
            this.TopicName = topicName;
        }

        public Type KeyType { get; }

        public Type ValueType { get; }

        public string TopicName { get; }
    }
}
