// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

using System;
using System.Data.SqlClient;
using System.Reflection;

using Gems.OpenTelemetry.Configuration;
using Gems.OpenTelemetry.GlobalOptions;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Primitives;

using Npgsql;

using OpenTelemetry;
using OpenTelemetry.Exporter;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

namespace Gems.OpenTelemetry;

public static class ServiceCollectionExtensions
{
    public static OpenTelemetryBuilder WithDefaultTracing(
        this OpenTelemetryBuilder openTelemetry,
        Action<TracingBuilder> configure = default)
    {
        var tracerBuilder = new TracingBuilder();
        configure?.Invoke(tracerBuilder);

        TracingGlobalOptions.Enabled = tracerBuilder.EnableTracing ?? TracingGlobalOptions.Enabled;

        var serviceName = string.IsNullOrEmpty(tracerBuilder.ServiceName) ?
            Assembly.GetEntryAssembly()?.GetName().Name :
            tracerBuilder.ServiceName;
        var serviceVersion = string.IsNullOrEmpty(tracerBuilder.ServiceVersion) ?
            Assembly.GetEntryAssembly()?.GetName().Version?.ToString() :
            tracerBuilder.ServiceVersion;

        TracingGlobalOptions.ServiceName = serviceName!;

        return openTelemetry
            .WithTracing(tb =>
            {
                if (!string.IsNullOrEmpty(serviceName))
                {
                    tb.AddSource(serviceName);
                }

                tb
                    .AddHttpClientInstrumentation(o =>
                    {
                        tracerBuilder.HttpClientConfigurator?.Invoke(o);
                    })
                    .AddAspNetCoreInstrumentation(o =>
                    {
                        tracerBuilder.AspNetCoreConfigurator?.Invoke(o);
                    })
                    .AddNpgsql(o => tracerBuilder.NpgsqlConfigurator?.Invoke(o))
                    .AddSqlClientInstrumentation(o => tracerBuilder.MssqlConfigurator?.Invoke(o));

                tb.AddOtlpExporter(otlp =>
                {
                    EnvironmentConfigurator.Configure(otlp);
                    tracerBuilder.OtlpExporterConfigurator?.Invoke(otlp);
                });

                if (!string.IsNullOrEmpty(serviceName) && !string.IsNullOrEmpty(serviceVersion))
                {
                    tb.ConfigureResource(rb => rb.AddService(
                        serviceName: serviceName,
                        serviceVersion: serviceVersion,
                        serviceInstanceId: Environment.MachineName));
                }

                tb.SetSampler(new TracingSampler());
                tracerBuilder.TracerProviderConfigurator?.Invoke(tb);
            });
    }

    public static OpenTelemetryBuilder AddDefaultTracing(
        this IServiceCollection services,
        Action<TracingBuilder> configure = default)
    {
        return services
            .AddOpenTelemetry()
            .WithDefaultTracing(configure);
    }

    public static OpenTelemetryBuilder AddDefaultTracing(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var tracingConfiguration = configuration.GetAndUpdateTracingConfiguration();
        ChangeToken.OnChange<object>(configuration.GetReloadToken, _ => configuration.GetAndUpdateTracingConfiguration(), null);
        services.AddHostedService<TracingFeatureFlagChecker>();

        return services
            .AddOpenTelemetry()
            .WithDefaultTracing(tb =>
            {
                tb.EnableTracing = tracingConfiguration?.Enabled;
                tb.ServiceName = tracingConfiguration?.ServiceName;
                tb.ServiceVersion = tracingConfiguration?.ServiceVersion;
                tb.OtlpExporterConfigurator = o =>
                {
                    if (!string.IsNullOrEmpty(tracingConfiguration?.Endpoint))
                    {
                        o.Endpoint = new Uri(tracingConfiguration.Endpoint);
                    }
                };

                if (tracingConfiguration?.Mssql != null)
                {
                    var mssqlOptions = tracingConfiguration.Mssql;
                    tb.MssqlConfigurator = o =>
                    {
                        o.SetDbStatementForStoredProcedure = mssqlOptions.SetDbStatementForStoredProcedure ?? o.SetDbStatementForStoredProcedure;
                        o.SetDbStatementForText = mssqlOptions.SetDbStatementForText ?? o.SetDbStatementForText;
                        o.RecordException = mssqlOptions.RecordException ?? o.RecordException;
                        o.Filter = static cmd => cmd is SqlCommand sqlCommand ? TracingGlobalOptions.MssqlCommandFilter.RxMatch(sqlCommand.CommandText) : false;
                    };
                }

                tb.AspNetCoreConfigurator = o =>
                {
                    o.Filter = static ctx => TracingGlobalOptions.RequestInUrlFilter.WildcardMatch(ctx.Request.Path);
                };

                tb.HttpClientConfigurator = o =>
                {
                    o.FilterHttpRequestMessage = static message => TracingGlobalOptions.RequestOutUrlFilter.WildcardMatch(message.RequestUri?.ToString() ?? string.Empty);
                };

                tb.TracerProviderConfigurator = o =>
                {
                    if (tracingConfiguration?.Console == true)
                    {
                        o.AddConsoleExporter(ceo =>
                        {
                            ceo.Targets = ConsoleExporterOutputTargets.Console | ConsoleExporterOutputTargets.Debug;
                        });
                    }
                };
            });
    }
}
