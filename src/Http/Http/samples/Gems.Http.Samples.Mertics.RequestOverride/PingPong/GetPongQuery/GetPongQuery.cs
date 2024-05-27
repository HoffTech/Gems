// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

using MediatR;

using Microsoft.AspNetCore.Mvc;

namespace Gems.Http.Samples.Mertics.RequestOverride.PingPong.GetPongQuery;

public class GetPongQuery : IRequest<string>
{
    [FromQuery(Name = "secret")]
    public string Secret { get; set; }
}
