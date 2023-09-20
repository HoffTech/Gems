// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

using Gems.Logging.Mvc.LogsCollector;
using Gems.Mvc.GenericControllers;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.Extensions.Logging;

namespace Gems.Logging.Mvc.Middlewares
{
    public class EndpointLoggingMiddleware
    {
        private readonly RequestDelegate next;
        private readonly ILogger<EndpointLoggingMiddleware> logger;
        private readonly IRequestLogsCollectorFactory logsCollectorFactory;

        public EndpointLoggingMiddleware(
            RequestDelegate next,
            ILogger<EndpointLoggingMiddleware> logger,
            IRequestLogsCollectorFactory logsCollectorFactory)
        {
            this.next = next;
            this.logger = logger;
            this.logsCollectorFactory = logsCollectorFactory;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            // не логируем эндпоинты для метрик и хелсчека
            if (context.Request.Path == PathString.FromUriComponent("/metrics")
                || context.Request.Path == PathString.FromUriComponent("/health")
                || context.Request.Path == PathString.FromUriComponent("/liveness")
                || context.Request.Path == PathString.FromUriComponent("/readiness"))
            {
                await this.next(context);
                return;
            }

            // не логируем эндпоинты от генерик контроллеров
            var controllerType = context?.GetEndpoint()?.Metadata?.GetMetadata<ControllerActionDescriptor>()?.MethodInfo.DeclaringType;
            if (controllerType != null && ControllerRegister.ControllerInfos.TryGetValue(controllerType, out _))
            {
                await this.next(context);
                return;
            }

            if (!context.Request.Body.CanSeek)
            {
                context.Request.EnableBuffering();
            }

            context.Request.Body.Position = 0;
            using var requestReader = new StreamReader(context.Request.Body);
            var requestBody = await requestReader.ReadToEndAsync().ConfigureAwait(false);
            context.Request.Body.Position = 0;

            var responseStream = context.Response.Body;
            try
            {
                await using var memoryStream = new MemoryStream();
                context.Response.Body = memoryStream;
                var sw = new Stopwatch();
                sw.Start();

                await this.next(context);

                sw.Stop();
                memoryStream.Position = 0;
                var responseBody = await new StreamReader(memoryStream).ReadToEndAsync();

                this.WriteLogs(context, requestBody, responseBody, sw.Elapsed.Milliseconds);
                memoryStream.Position = 0;
                await memoryStream.CopyToAsync(responseStream);
            }
            finally
            {
                context.Response.Body = responseStream;
            }
        }

        private void WriteLogs(HttpContext context, string requestBody, string responseBody, long requestDuration)
        {
            var logsCollector = this.logsCollectorFactory.Create(this.logger);
            logsCollector.AddPath(context.Request.Path);
            logsCollector.AddRequest(requestBody);
            logsCollector.AddRequestHeaders(context.Request.Headers.ToDictionary(x => x.Key, y => string.Join(',', y.Value)));
            logsCollector.AddStatus(context.Response.StatusCode);
            logsCollector.AddResponseHeaders(context.Response.Headers.ToDictionary(x => x.Key, y => string.Join(',', y.Value)));
            logsCollector.AddResponse(responseBody);
            logsCollector.AddRequestDuration(requestDuration);
            logsCollector.WriteLogs();
        }
    }
}
