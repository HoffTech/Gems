// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

using System.Collections.Generic;

namespace Gems.MessageBrokers.Kafka.Entities;

public interface IHasMessageMetadata<TKey>
{
    Dictionary<string, string> Headers { get; set; }

    TKey Key { get; set; }

    int Partition { get; set; }

    long Offset { get; set; }
}
