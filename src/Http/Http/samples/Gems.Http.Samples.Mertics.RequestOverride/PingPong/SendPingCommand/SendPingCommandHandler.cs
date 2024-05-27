// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

using System.Threading;
using System.Threading.Tasks;

using Gems.Http.Samples.Mertics.RequestOverride.PingPong.SendPingCommand.PongApi;
using Gems.Mvc.GenericControllers;

using MediatR;

namespace Gems.Http.Samples.Mertics.RequestOverride.PingPong.SendPingCommand;

[Endpoint(
    "api/v1/samples/ping",
    "GET",
    OperationGroup = "Samples",
    Summary = "Отправляет запрос на получение pong.")]
public class SendPingCommandHandler(PongService pongService)
    : IRequestHandler<SendPingCommand, string>
{
    public Task<string> Handle(SendPingCommand command, CancellationToken cancellationToken)
    {
        return pongService.GetPong(cancellationToken);
    }
}
