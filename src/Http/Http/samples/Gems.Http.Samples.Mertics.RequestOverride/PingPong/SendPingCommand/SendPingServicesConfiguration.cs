// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

using Gems.Http.Samples.Mertics.RequestOverride.PingPong.SendPingCommand.PongApi;
using Gems.Mvc;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Gems.Http.Samples.Mertics.RequestOverride.PingPong.SendPingCommand;

public class SendPingServicesConfiguration : IServicesConfiguration
{
    public void Configure(IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton<PongService>();
    }
}
