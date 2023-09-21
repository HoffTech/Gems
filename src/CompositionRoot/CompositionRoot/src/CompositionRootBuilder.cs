// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

using System;
using System.Linq;

using Gems.Caching;
using Gems.Caching.Behaviors;
using Gems.Data.Behaviors;
using Gems.Data.Npgsql;
using Gems.Data.SqlServer;
using Gems.HealthChecks;
using Gems.Http;
using Gems.Jobs.Quartz;
using Gems.Logging.Mvc;
using Gems.Logging.Mvc.Behaviors;
using Gems.Metrics.Behaviors;
using Gems.Metrics.Prometheus;
using Gems.Mvc;
using Gems.Mvc.Behaviors;
using Gems.Swagger;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Gems.CompositionRoot;

public class CompositionRootBuilder<TFromAssemblyContaining>
{
    private readonly IConfiguration configuration;
    private readonly IServiceCollection services;
    private readonly Action<CompositionRootOptions> configureOptions;

    public CompositionRootBuilder(IServiceCollection services, IConfiguration configuration, Action<CompositionRootOptions> configureOptions = null)
    {
        this.configuration = configuration;
        this.services = services;
        this.configureOptions = configureOptions;
    }

    public void Build()
    {
        var options = new CompositionRootOptions();
        this.configureOptions?.Invoke(options);

        (options.AddControllersWithMediatR ?? this.AddControllersWithMediatR)();
        (options.AddPrometheus ?? this.AddPrometheus)();
        (options.AddHealthChecks ?? this.AddHealthChecks)();
        (options.AddSwagger ?? this.AddSwagger)();
        (options.AddAutoMapper ?? this.AddAutoMapper)();
        (options.AddMediatR ?? this.AddMediatR)();
        (options.AddPipelines ?? this.AddPipelines)();
        (options.AddValidation ?? this.AddValidation)();
        (options.AddJobs ?? this.AddJobs)();
        (options.AddUnitOfWorks ?? this.AddUnitOfWorks)();
        (options.AddHttpServices ?? this.AddHttpServices)();
        (options.AddDistributedCache ?? this.AddDistributedCache)();
        (options.RegisterServices ?? this.RegisterServices)();
        (options.AddSecureLogging ?? this.AddSecureLogging)();
    }

    private void AddControllersWithMediatR()
    {
        var hideEndpointStartWith = this.configuration.GetValue<string>("HideEndpointStartWith");
        this.services.AddControllersWithMediatR(
            options => options.RegisterControllersFromAssemblyContaining<TFromAssemblyContaining>(hideEndpointStartWith),
            this.configuration);
    }

    private void AddPrometheus()
    {
        this.services.AddPrometheus(this.configuration);
    }

    private void AddHealthChecks()
    {
        this.services.AddDefaultHealthChecks();
    }

    private void AddSwagger()
    {
        this.services.AddSwagger(this.configuration);
    }

    private void AddAutoMapper()
    {
        this.services.AddAutoMapper(typeof(TFromAssemblyContaining).Assembly);
    }

    private void AddMediatR()
    {
        this.services.AddMediatR(config => config.RegisterServicesFromAssemblyContaining<TFromAssemblyContaining>());
    }

    private void AddPipelines()
    {
        this.services.AddHttpContextAccessor(); // для EndpointLoggingBehavior.
        this.services.AddPipeline(typeof(EndpointLoggingBehavior<,>));
        this.services.AddPipeline(typeof(NotFoundBehavior<,>));
        this.services.AddPipeline(typeof(ExceptionBehavior<,>));
        this.services.AddPipeline(typeof(CacheBehavior<,>));
        this.services.AddPipeline(typeof(ResetMetricsBehavior<,>));
        this.services.AddPipeline(typeof(ErrorMetricsBehavior<,>));
        this.services.AddPipeline(typeof(TimeMetricBehavior<,>));
        this.services.AddPipeline(typeof(ValidatorBehavior<,>));
        this.services.AddPipeline(typeof(UnitOfWorkBehavior<,>));
    }

    private void AddValidation()
    {
        this.services.AddValidation(options => options.RegisterValidatorsFromAssemblyContaining<TFromAssemblyContaining>());
    }

    private void AddJobs()
    {
        this.services.AddQuartzWithJobs(this.configuration, options => options.RegisterJobsFromAssemblyContaining<TFromAssemblyContaining>());
    }

    private void AddUnitOfWorks()
    {
        var postgreSqlUnitOfWorkKeys = this.configuration.GetSection(PostgresqlUnitOfWorkOptionsList.Name).Get<PostgresqlUnitOfWorkOptionsList>()?.Select(x => x.Key) ?? Array.Empty<string>();
        foreach (var key in postgreSqlUnitOfWorkKeys)
        {
            this.services.AddPostgresqlUnitOfWork(this.configuration, key, options => options.RegisterMappersFromAssemblyContaining<TFromAssemblyContaining>());
        }

        var msSqlUnitOfWorkKeys = this.configuration.GetSection(MsSqlUnitOfWorkOptionsList.Name).Get<MsSqlUnitOfWorkOptionsList>()?.Select(x => x.Key) ?? Array.Empty<string>();
        foreach (var key in msSqlUnitOfWorkKeys)
        {
            this.services.AddMsSqlUnitOfWork(this.configuration, key, options => options.RegisterMappersFromAssemblyContaining<TFromAssemblyContaining>());
        }
    }

    private void AddHttpServices()
    {
        this.services.AddHttpServices(this.configuration);
    }

    private void AddDistributedCache()
    {
        this.services.AddDistributedCache(this.configuration);
    }

    private void RegisterServices()
    {
        this.services.RegisterServicesFromAssemblyContaining<TFromAssemblyContaining>(this.configuration);
    }

    private void AddSecureLogging()
    {
        this.services.AddSecureLogging();
    }
}
