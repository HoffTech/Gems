// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

using System;
using System.Threading;
using System.Threading.Tasks;

using Gems.OpenTelemetry.Api.Dto;
using Gems.OpenTelemetry.Configuration;
using Gems.OpenTelemetry.GlobalOptions;
using Gems.Settings.Gitlab;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Gems.OpenTelemetry;

public class TracingFeatureFlagChecker : BackgroundService
{
    private readonly TracingFeatureFlags flags;
    private readonly IServiceProvider serviceProvider;
    private readonly TracingConfiguration configuration;
    private readonly ILogger<TracingFeatureFlagChecker> logger;
    private readonly string environmentPrefix;
    private bool wasEnabled;

    public TracingFeatureFlagChecker(
        TracingFeatureFlags flags,
        IServiceProvider serviceProvider,
        ILogger<TracingFeatureFlagChecker> logger,
        TracingConfiguration configuration)
    {
        this.flags = flags;
        this.serviceProvider = serviceProvider;
        this.logger = logger;
        this.configuration = configuration;

        this.environmentPrefix = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
        if (string.IsNullOrEmpty(this.environmentPrefix))
        {
            this.environmentPrefix = Environment.GetEnvironmentVariable("DOTNET_ENVIRONMENT");
        }
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        GitlabConfigurationValuesProvider gitlabConfigurationProvider;
        try
        {
            gitlabConfigurationProvider = this.serviceProvider.GetRequiredService<GitlabConfigurationValuesProvider>();
        }
        catch (Exception)
        {
            this.logger.LogError($"Can't find {nameof(GitlabConfigurationValuesProvider)}. {nameof(TracingFeatureFlagChecker)} stopped");
            return;
        }

        while (!stoppingToken.IsCancellationRequested)
        {
            if (this.FeatureEnabled() && !this.wasEnabled)
            {
                TraceRequest request;
                try
                {
                    request = await gitlabConfigurationProvider.GetGitlabVariableValueByName<TraceRequest>("tracing");
                }
                catch (Exception)
                {
                    this.logger.LogError("Can't deserialize gitlab's variable \"tracing\"");
                    await Task.Delay(TimeSpan.FromSeconds(this.configuration.FeatureFlagUpdateIntervalOnFailureSeconds ?? 300), stoppingToken);
                    continue;
                }

                TracingGlobalOptions.Enabled = request?.Enabled ?? TracingGlobalOptions.Enabled;
                TracingGlobalOptions.RequestInUrlFilter.Include = request?.RequestIn?.Include ?? TracingGlobalOptions.RequestInUrlFilter.Include;
                TracingGlobalOptions.RequestInUrlFilter.Exclude = request?.RequestIn?.Exclude ?? TracingGlobalOptions.RequestInUrlFilter.Exclude;
                TracingGlobalOptions.RequestOutUrlFilter.Include = request?.RequestOut?.Include ?? TracingGlobalOptions.RequestOutUrlFilter.Include;
                TracingGlobalOptions.RequestOutUrlFilter.Exclude = request?.RequestOut?.Exclude ?? TracingGlobalOptions.RequestOutUrlFilter.Exclude;
                TracingGlobalOptions.SourceFilter.Include = request?.SourceFilter?.Include ?? TracingGlobalOptions.SourceFilter.Include;
                TracingGlobalOptions.SourceFilter.Exclude = request?.SourceFilter?.Exclude ?? TracingGlobalOptions.SourceFilter.Exclude;
                TracingGlobalOptions.MssqlCommandFilter.Include = request?.Mssql?.CommandFilter?.Include ?? TracingGlobalOptions.MssqlCommandFilter.Include;
                TracingGlobalOptions.MssqlCommandFilter.Exclude = request?.Mssql?.CommandFilter?.Exclude ?? TracingGlobalOptions.MssqlCommandFilter.Exclude;
                TracingGlobalOptions.IncludeCommandRequest = request?.Command?.IncludeRequest ?? TracingGlobalOptions.IncludeCommandRequest;
                TracingGlobalOptions.IncludeCommandResponse = request?.Command?.IncludeResponse ?? TracingGlobalOptions.IncludeCommandResponse;

                this.logger.LogInformation("Trace configuration updated successfully");
                this.wasEnabled = true;
            }
            else
            {
                if (!this.wasEnabled)
                {
                    continue;
                }

                this.logger.LogInformation("Tracing disabled via feature flag");
                this.wasEnabled = false;
            }
        }
    }

    private bool FeatureEnabled()
    {
        return this.environmentPrefix switch
        {
            "Development" => this.flags.DevTracing || this.flags.Tracing,
            "Staging" => this.flags.StageTracing || this.flags.Tracing,
            "Production" => this.flags.ProdTracing || this.flags.Tracing,
            _ => false
        };
    }
}
