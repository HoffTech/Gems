// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

namespace Gems.Patterns.ProducerConsumer
{
    public class ProducerConsumerOptions
    {
        public int MaxAttempts { get; set; }

        public int DelayBetweenAttemptsInMilliseconds { get; set; }
    }
}
