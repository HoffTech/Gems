// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

using System;
using System.Threading;
using System.Threading.Tasks;

using Gems.OpenTelemetry.Api.Dto;
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
    private readonly ILogger<TracingFeatureFlagChecker> logger;

    public TracingFeatureFlagChecker(TracingFeatureFlags flags, IServiceProvider serviceProvider, ILogger<TracingFeatureFlagChecker> logger)
    {
        this.flags = flags;
        this.serviceProvider = serviceProvider;
        this.logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        GitlabConfigurationValuesProvider gitlabConfigurationProvider;
        try
        {
            gitlabConfigurationProvider = this.serviceProvider.GetRequiredService<GitlabConfigurationValuesProvider>();
        }
        catch (Exception e)
        {
            this.logger.LogError($"Can't find {nameof(GitlabConfigurationValuesProvider)}. {nameof(TracingFeatureFlagChecker)} stopped");
            return;
        }

        while (!stoppingToken.IsCancellationRequested)
        {
            if (this.flags.Tracing)
            {
                var request = await gitlabConfigurationProvider.GetGitlabVariableValueByName<TraceRequest>("tracing");

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

            await Task.Delay(TimeSpan.FromMinutes(5), stoppingToken);
        }
    }
}
