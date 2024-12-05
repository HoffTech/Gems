// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

using Gems.Logging.Mvc.LogsCollector;
using Gems.Metrics;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Gems.Http;

public class BaseClientServiceHelper
{
    public BaseClientServiceHelper(
        IMetricsService metricsService,
        ILogger<BaseClientServiceHelper> logger,
        IRequestLogsCollectorFactory logsCollectorFactory,
        IOptions<RequestLogsCollectorOptions> requestLogsCollectorOptions,
        IHttpClientFactory httpClientFactory,
        IConfiguration configuration)
    {
        this.MetricsService = metricsService;
        this.Logger = logger;
        this.LogsCollectorFactory = logsCollectorFactory;
        this.RequestLogsCollectorOptions = requestLogsCollectorOptions;
        this.HttpClientFactory = httpClientFactory;
        this.Configuration = configuration;
    }

    public IMetricsService MetricsService { get; }

    public ILogger<BaseClientServiceHelper> Logger { get; }

    public IRequestLogsCollectorFactory LogsCollectorFactory { get; }

    public IOptions<RequestLogsCollectorOptions> RequestLogsCollectorOptions { get; }

    public IHttpClientFactory HttpClientFactory { get; }

    public IConfiguration Configuration { get; }
}
