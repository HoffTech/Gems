// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

using Gems.Http.Samples.UseTemplateUri.PingPong.SendPingCommand.PongApi;
using Gems.Mvc.GenericControllers;

using MediatR;

namespace Gems.Http.Samples.UseTemplateUri.PingPong.SendPingCommand;

[Endpoint("v1/Samples/UseTemplateUri/ping", "GET", OperationGroup = "Samples", Summary = "Отправляет запрос на получение pong.")]
public class SendPingCommandHandler : IRequestHandler<SendPingCommand, string>
{
    private readonly PongService pongService;

    public SendPingCommandHandler(PongService pongService)
    {
        this.pongService = pongService;
    }

    public Task<string> Handle(SendPingCommand command, CancellationToken cancellationToken)
    {
        return this.pongService.GetPong(cancellationToken);
    }
}
