// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

using MediatR;

using Microsoft.AspNetCore.Mvc;

namespace Gems.Http.Samples.UseTemplateUri.PingPong.GetPongQuery;

public class GetPongQuery : IRequest<string>
{
    [FromRoute(Name = "secret")]
    public string Secret { get; set; }
}
