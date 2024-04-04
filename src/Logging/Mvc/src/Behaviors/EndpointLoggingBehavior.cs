// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Gems.Logging.Mvc.LogsCollector;
using Gems.Mvc;
using Gems.Mvc.Filters.Errors;
using Gems.Mvc.GenericControllers;

using MediatR;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.Extensions.Logging;

namespace Gems.Logging.Mvc.Behaviors
{
    public class EndpointLoggingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
        where TRequest : IRequestEndpointLogging
    {
        private readonly IHttpContextAccessor httpContextAccessor;
        private readonly ILogger<EndpointLoggingBehavior<TRequest, TResponse>> logger;
        private readonly IRequestLogsCollectorFactory logsCollectorFactory;
        private readonly IConverter<Exception, BusinessErrorViewModel> exceptionConverter;

        public EndpointLoggingBehavior(
            IHttpContextAccessor httpContextAccessor,
            ILogger<EndpointLoggingBehavior<TRequest, TResponse>> logger,
            IRequestLogsCollectorFactory logsCollectorFactory,
            IConverter<Exception, BusinessErrorViewModel> exceptionConverter)
        {
            this.httpContextAccessor = httpContextAccessor;
            this.logger = logger;
            this.logsCollectorFactory = logsCollectorFactory;
            this.exceptionConverter = exceptionConverter;
        }

        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        {
            var sw = new Stopwatch();
            sw.Start();
            var context = this.httpContextAccessor.HttpContext;
            var logsCollector = this.logsCollectorFactory.Create(this.logger);
            this.AddEndpointLogs(context, logsCollector);
            logsCollector.AddLogsFromPayload(request);
            logsCollector.AddRequest(request);
            try
            {
                var response = await next();
                if (response is Unit)
                {
                    return response;
                }

                logsCollector.AddLogsFromPayload(response);
                logsCollector.AddResponse(response);
                logsCollector.AddStatus(200);

                return response;
            }
            catch (Exception exception)
            {
                var error = this.exceptionConverter.Convert(exception);
                logsCollector.AddResponse(error);
                logsCollector.AddStatus(error.StatusCode ?? 499);
                throw;
            }
            finally
            {
                if (context != null)
                {
                    logsCollector.AddResponseHeaders(context.Response.Headers.ToDictionary(x => x.Key, y => string.Join(',', y.Value)));
                }

                sw.Stop();
                logsCollector.AddRequestDuration(sw.Elapsed.TotalMilliseconds);
                logsCollector.WriteLogs();
            }
        }

        private void AddEndpointLogs(HttpContext context, RequestLogsCollector logsCollector)
        {
            if (context == null)
            {
                return;
            }

            logsCollector.AddPath(context.Request.Path);
            logsCollector.AddRequestHeaders(context.Request.Headers.ToDictionary(x => x.Key, y => string.Join(',', y.Value)));

            var controllerType = context.GetEndpoint()?.Metadata?.GetMetadata<ControllerActionDescriptor>()?.MethodInfo.DeclaringType;
            if (controllerType == null || !ControllerRegister.ControllerInfos.TryGetValue(controllerType, out var endpoint))
            {
                return;
            }

            if (endpoint.Summary != null)
            {
                logsCollector.AddEndpointSummary(endpoint.Summary);
            }
        }
    }
}
