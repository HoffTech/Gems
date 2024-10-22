// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

using Gems.Logging.Mvc.LogsCollector;
using Gems.Metrics;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Gems.Http;

public class BaseClientServiceHelper
{
    public BaseClientServiceHelper(
        IMetricsService metricsService,
        ILogger<BaseClientServiceHelper> logger,
        IRequestLogsCollectorFactory logsCollectorFactory,
        IHttpClientFactory httpClientFactory,
        IConfiguration configuration)
    {
        this.Configuration = configuration;
        this.MetricsService = metricsService;
        this.Logger = logger;
        this.LogsCollectorFactory = logsCollectorFactory;
        this.HttpClientFactory = httpClientFactory;
    }

    public IMetricsService MetricsService { get; }

    public ILogger<BaseClientServiceHelper> Logger { get; }

    public IRequestLogsCollectorFactory LogsCollectorFactory { get; }

    public IHttpClientFactory HttpClientFactory { get; }

    public IConfiguration Configuration { get; }
}
