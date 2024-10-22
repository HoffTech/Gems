// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

using Gems.Caching.Behaviors;
using Gems.Data.Npgsql;
using Gems.HealthChecks;
using Gems.Http;
using Gems.Logging.Mvc;
using Gems.Logging.Mvc.Behaviors;
using Gems.Logging.Mvc.Middlewares;
using Gems.Metrics.Behaviors;
using Gems.Metrics.Prometheus;
using Gems.Mvc;
using Gems.Mvc.Behaviors;
using Gems.Mvc.Filters.Errors;
using Gems.Swagger;

using NLog.Web;

using Prometheus;

var builder = WebApplication.CreateBuilder(args);

builder.Host
    .ConfigureLogging(logging => logging.ClearProviders())
    .UseNLog();

// Add services to the container.
builder.Services.AddHttpContextAccessor();
builder.Services.AddSecureLogging();
builder.Services.AddDefaultHealthChecks();
builder.Services.AddMediatR(configuration => configuration.RegisterServicesFromAssemblyContaining<Gems.TestInfrastructure.Samples.WeatherInfo.Program>());

builder.Services.AddControllersWithMediatR(options => options.RegisterControllersFromAssemblyContaining<Gems.TestInfrastructure.Samples.WeatherInfo.Program>());
builder.Services.AddSwagger(builder.Configuration, typeof(BusinessErrorViewModel), typeof(BusinessErrorViewModel));

builder.Services.AddPipeline(typeof(EndpointLoggingBehavior<,>));
builder.Services.AddPipeline(typeof(ScopeLoggingBehavior<,>));
builder.Services.AddPipeline(typeof(NotFoundBehavior<,>));
builder.Services.AddPipeline(typeof(ExceptionBehavior<,>));
builder.Services.AddPipeline(typeof(CacheBehavior<,>));
builder.Services.AddPipeline(typeof(ErrorMetricsBehavior<,>));
builder.Services.AddPipeline(typeof(TimeMetricBehavior<,>));
builder.Services.AddPipeline(typeof(ValidatorBehavior<,>));
builder.Services.AddValidation(options => options.RegisterValidatorsFromAssemblyContaining<Gems.TestInfrastructure.Samples.WeatherInfo.Program>());
builder.Services.AddPrometheus(builder.Configuration);
builder.Services.AddHttpServices(builder.Configuration);
builder.Services.RegisterServicesFromAssemblyContaining<Gems.TestInfrastructure.Samples.WeatherInfo.Program>(builder.Configuration);

builder.Services.AddPostgresqlUnitOfWork("DefaultConnection", options =>
{
    options.ConnectionString = builder.Configuration.GetConnectionString("DefaultConnection");
    options.RegisterMappersFromAssemblyContaining<Gems.TestInfrastructure.Samples.WeatherInfo.Program>();
});

builder.Services.AddControllers();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseRouting();
app.UseHttpMetrics();
app.UseAuthentication();
app.UseAuthorization();
app.UseSwaggerApi(builder.Configuration);
app.UseEndpointLogging();
app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
    endpoints.MapMetrics();
    endpoints.MapDefaultHealthChecks();
});
app.Run();
