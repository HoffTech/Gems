// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

using Gems.CompositionRoot;
using Gems.HealthChecks;
using Gems.Logging.Mvc.Behaviors;
using Gems.Metrics.Behaviors;
using Gems.Mvc;
using Gems.Mvc.Behaviors;
using Gems.Swagger;

using Prometheus;

namespace Gems.Patterns.SyncTables.Sample.SyncTables;

public class Startup(IConfiguration configuration)
{
    public void ConfigureServices(IServiceCollection services)
    {
        services.ConfigureCompositionRoot<Startup>(
            configuration,
            opt =>
            {
                opt.AddPipelines = () =>
                {
                    services.AddHttpContextAccessor();
                    services.AddPipeline(typeof(ScopeLoggingBehavior<,>));
                    services.AddPipeline(typeof(EndpointLoggingBehavior<,>));
                    services.AddPipeline(typeof(NotFoundBehavior<,>));
                    services.AddPipeline(typeof(ExceptionBehavior<,>));
                    services.AddPipeline(typeof(ErrorMetricsBehavior<,>));
                    services.AddPipeline(typeof(TimeMetricBehavior<,>));
                    services.AddPipeline(typeof(ValidatorBehavior<,>));
                };
            });
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }

        app.UseRouting();
        app.UseHttpMetrics();
        app.UseAuthentication();
        app.UseAuthorization();
        app.UseSwaggerApi(configuration);
        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();
            endpoints.MapMetrics();
            endpoints.MapDefaultHealthChecks();
        });
    }
}
