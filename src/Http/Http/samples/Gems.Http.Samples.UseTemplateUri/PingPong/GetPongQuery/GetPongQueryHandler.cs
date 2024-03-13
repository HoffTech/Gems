// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

using Gems.Mvc.GenericControllers;

using MediatR;

namespace Gems.Http.Samples.UseTemplateUri.PingPong.GetPongQuery;

[Endpoint("v1/Samples/UseTemplateUri/{secret}/pong", "GET", OperationGroup = "Samples", Summary = "Возращает pong.")]
public class GetPongQueryHandler : IRequestHandler<GetPongQuery, string>
{
    public Task<string> Handle(GetPongQuery query, CancellationToken cancellationToken)
    {
        if (query.Secret != "ping")
        {
            throw new InvalidOperationException("Отправьте команду ping.");
        }

        return Task.FromResult("Pong");
    }
}
