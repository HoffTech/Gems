// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

using System.Threading;
using System.Threading.Tasks;

using Gems.Mvc.Filters.Exceptions;
using Gems.Mvc.GenericControllers;

using MediatR;

namespace Gems.Http.Samples.Mertics.RequestOverride.PingPong.GetPongQuery;

[Endpoint("api/v1/samples/pong", "GET", OperationGroup = "Samples", Summary = "Возращает pong.")]
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
