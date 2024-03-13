// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

using Gems.Mvc;

namespace Gems.Patterns.ProducerConsumer.SampleUsing.Persons.SyncPersons;

public class SyncPersonsServicesConfiguration : IServicesConfiguration
{
    public void Configure(IServiceCollection services, IConfiguration configuration)
    {
        services.AddProducerConsumerPattern(_ => configuration.GetSection("ProducerConsumerOptions").Get<ProducerConsumerOptions>());
    }
}
