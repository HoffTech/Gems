// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

using System;

namespace Gems.CompositionRoot;

public class CompositionRootOptions
{
    public Action AddControllersWithMediatR { get; set; }

    public Action AddPrometheus { get; set; }

    public Action AddHealthChecks { get; set; }

    public Action AddSwagger { get; set; }

    public Action AddAutoMapper { get; set; }

    public Action AddMediatR { get; set; }

    public Action AddPipelines { get; set; }

    public Action AddValidation { get; set; }

    public Action AddJobs { get; set; }

    public Action AddUnitOfWorks { get; set; }

    public Action AddHttpServices { get; set; }

    public Action AddDistributedCache { get; set; }

    public Action RegisterServices { get; set; }

    public Action AddSecureLogging { get; set; }

    public Action AddProducers { get; set; }

    public Action AddConsumers { get; set; }
}
