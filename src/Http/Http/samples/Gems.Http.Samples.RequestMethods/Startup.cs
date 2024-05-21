// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

using Autofac;

using Gems.CompositionRoot;
using Gems.Swagger;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Gems.Http.Samples.RequestMethods;

public class Startup(IConfiguration configuration)
{
    public void ConfigureServices(IServiceCollection services)
    {
        services.ConfigureCompositionRoot<Startup>(
            configuration,
            opt =>
            {
                opt.AddHttpServices = () => services.AddHttpServices(configuration);
                opt.AddPipelines = () => { };
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
        });
    }
}
