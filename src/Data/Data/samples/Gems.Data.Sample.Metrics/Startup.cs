// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

using Autofac;

using Gems.CompositionRoot;
using Gems.Data.Behaviors;
using Gems.Data.Npgsql;
using Gems.Mvc;
using Gems.Swagger;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using Prometheus;

namespace Gems.Data.Sample.Metrics;

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
                    services.AddPipeline(typeof(UnitOfWorkBehavior<,>));
                };

                opt.AddUnitOfWorks = () =>
                {
                    services.AddPostgresqlUnitOfWork(
                        "default",
                        options =>
                        {
                            options.ConnectionString = configuration.GetConnectionString("DefaultConnection");
                            options.RegisterMappersFromAssemblyContaining<Startup>();
                            options.DbQueryMetricType = DbQueryMetricType.GemsDataSampleMetricsDbQueryTime;

                            // Альтернативная конфигурация
                            // options.DbQueryMetricInfo = new MetricInfo { Name = "sales_return_claim_db_query_time", Description = "Sales Return Claim Db Query Time" };
                        });
                };
            });
    }

    public void ConfigureContainer(ContainerBuilder builder)
    {
        builder.ConfigureAutofacCompositionRoot<Startup>();
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }

        app.UseRouting();
        app.UseAuthentication();
        app.UseAuthorization();
        app.UseSwaggerApi(configuration);
        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();
            endpoints.MapMetrics();
        });
    }
}
