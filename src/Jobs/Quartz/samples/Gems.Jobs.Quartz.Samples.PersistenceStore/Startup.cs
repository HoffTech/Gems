// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

using Gems.Caching;
using Gems.Caching.Behaviors;
using Gems.Data.Behaviors;
using Gems.Data.Npgsql;
using Gems.HealthChecks;
using Gems.Http;
using Gems.Logging.Mvc;
using Gems.Logging.Mvc.Middlewares;
using Gems.Metrics.Behaviors;
using Gems.Metrics.Prometheus;
using Gems.Mvc;
using Gems.Mvc.Behaviors;
using Gems.Mvc.Filters.Errors;
using Gems.Swagger;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using Prometheus;

namespace Gems.Jobs.Quartz.Samples.PersistenceStore
{
    public class Startup
    {
        private readonly IConfiguration configuration;

        public Startup(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllersWithMediatR(options => options.RegisterControllersFromAssemblyContaining<Startup>());
            services.AddPrometheus(this.configuration);

            services.AddSwagger(this.configuration, typeof(ValidationResultViewModel), typeof(GenericErrorViewModel));
            services.AddMediatR(configuration => configuration.RegisterServicesFromAssemblyContaining<Startup>());

            services.AddPipeline(typeof(NotFoundBehavior<,>));
            services.AddPipeline(typeof(ExceptionBehavior<,>));
            services.AddPipeline(typeof(CacheBehavior<,>));
            services.AddPipeline(typeof(ErrorMetricsBehavior<,>));
            services.AddPipeline(typeof(TimeMetricBehavior<,>));
            services.AddPipeline(typeof(ValidatorBehavior<,>));
            services.AddPipeline(typeof(UnitOfWorkBehavior<,>));

            services.AddQuartzWithJobs(
                this.configuration,
                options => JobRegister.RegisterJobs(typeof(Startup).Assembly));

            services.AddValidation(options => options.RegisterValidatorsFromAssemblyContaining<Startup>());

            services.AddPostgresqlUnitOfWork(options =>
            {
                options.RegisterMappersFromAssemblyContaining<Startup>();
            });

            services.AddHttpServices(this.configuration);
            services.AddDistributedCache(this.configuration);
            services.RegisterServicesFromAssemblyContaining<Startup>(this.configuration);
            services.AddHttpContextAccessor();
            services.AddSecureLogging();
            services.AddDefaultHealthChecks();
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
            app.UseSwaggerApi(this.configuration);
            app.UseEndpointLogging();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapMetrics();
                endpoints.MapDefaultHealthChecks();
            });
        }
    }
}
