// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

using System;

using Microsoft.Extensions.DependencyInjection;

namespace Gems.Patterns.ProducerConsumer
{
    public static class ServiceCollectionExtensions
    {
        public static void AddProducerConsumerPattern(this IServiceCollection services, Action<ProducerConsumerOptions> configureOptions)
        {
            services.Configure(configureOptions);
        }
    }
}
