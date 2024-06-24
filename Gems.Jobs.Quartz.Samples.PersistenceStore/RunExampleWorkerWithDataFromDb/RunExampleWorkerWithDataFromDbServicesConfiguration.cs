// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

using Gems.Jobs.Quartz.Jobs.JobTriggerFromDb;
using Gems.Mvc;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Gems.Jobs.Quartz.Samples.PersistenceStore.RunExampleWorkerWithDataFromDb;

public class RunExampleWorkerWithDataFromDbServicesConfiguration : IServicesConfiguration
{
    public void Configure(IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton<ITriggerDataProvider, TriggerDataProvider>();
    }
}
