// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

using System;
using System.Threading;
using System.Threading.Tasks;

using Gems.OpenTelemetry.GlobalOptions;
using Gems.Text.Json;

using MediatR;

using OpenTelemetry.Trace;

namespace Gems.OpenTelemetry.Mvc;

public class TracingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
{
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        TResponse response;
        var tracer = TracerProvider.Default.GetTracer(TracingGlobalOptions.ServiceName);
        using var span = tracer.StartActiveSpan(typeof(TRequest).FullName ?? string.Empty);
        span.SetAttribute("gems.request.type", typeof(TRequest).FullName ?? string.Empty);
        span.SetAttribute("gems.response.type", typeof(TResponse).FullName ?? string.Empty);
        if (TracingGlobalOptions.IncludeCommandRequest)
        {
            span.SetAttribute("gems.request.text", SafeSerialize(request));
        }

        try
        {
            response = await next();

            if (TracingGlobalOptions.IncludeCommandResponse)
            {
                span.SetAttribute("gems.response.text", SafeSerialize(response));
            }
        }
        catch (Exception e)
        {
            span.SetStatus(Status.Error);
            span.RecordException(e);
            throw;
        }

        return response;
    }

    private static string SafeSerialize(object obj)
    {
        try
        {
            return obj.Serialize();
        }
        catch
        {
            return string.Empty;
        }
    }
}
