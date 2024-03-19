// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

using System;
using System.Threading;
using System.Threading.Tasks;

using Gems.FeatureToggle;
using Gems.OpenTelemetry.Api.Dto;
using Gems.OpenTelemetry.Configuration;
using Gems.OpenTelemetry.GlobalOptions;
using Gems.Settings.Gitlab;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Gems.OpenTelemetry;

public class TracingFeatureFlagChecker : BackgroundService
{
    private readonly TracingFeatureFlags flags;
    private readonly IServiceProvider serviceProvider;
    private readonly TracingConfiguration configuration;
    private readonly ILogger<TracingFeatureFlagChecker> logger;
    private readonly TimeSpan tracingFeatureFlagUpdateDelay;
    private bool wasEnabled;

    public TracingFeatureFlagChecker(
        TracingFeatureFlags flags,
        IServiceProvider serviceProvider,
        ILogger<TracingFeatureFlagChecker> logger,
        TracingConfiguration configuration,
        IOptions<FeatureToggleOptions> featureToggleOptions)
    {
        this.flags = flags;
        this.serviceProvider = serviceProvider;
        this.logger = logger;
        this.configuration = configuration;
        this.tracingFeatureFlagUpdateDelay = featureToggleOptions.Value.FetchTogglesInterval == default ?
            TimeSpan.FromSeconds(30) : featureToggleOptions.Value.FetchTogglesInterval;
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
            if (this.flags.TracingEnabled)
            {
                if (this.wasEnabled)
                {
                    await Task.Delay(this.tracingFeatureFlagUpdateDelay, stoppingToken);
                    continue;
                }

                try
                {
                    var request = await gitlabConfigurationProvider.GetGitlabVariableValueByName<TraceRequest>("tracing");
                    UpdateTracingGlobalOptions(request);
                    this.logger.LogInformation("Trace configuration updated successfully");
                }
                catch (Exception e)
                {
                    this.logger.LogCritical(e, "Error while fetching gitlab's variable \"tracing\"");
                }

                this.wasEnabled = true;
            }
            else
            {
                if (this.wasEnabled)
                {
                    this.logger.LogInformation("Tracing disabled via feature flag");
                    this.wasEnabled = false;
                }

                await Task.Delay(this.tracingFeatureFlagUpdateDelay, stoppingToken);
            }
        }
    }

    private static void UpdateTracingGlobalOptions(TraceRequest request)
    {
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
    }
}
