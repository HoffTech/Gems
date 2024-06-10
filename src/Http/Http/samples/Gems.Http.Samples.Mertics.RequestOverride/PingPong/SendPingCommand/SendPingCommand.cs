﻿// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

using MediatR;

namespace Gems.Http.Samples.Mertics.RequestOverride.PingPong.SendPingCommand;

public record SendPingCommand : IRequest<string>
{
}