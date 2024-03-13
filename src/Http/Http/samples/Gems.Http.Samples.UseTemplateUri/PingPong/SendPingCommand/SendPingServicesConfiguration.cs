// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

using Gems.Http.Samples.UseTemplateUri.PingPong.SendPingCommand.PongApi;
using Gems.Mvc;

namespace Gems.Http.Samples.UseTemplateUri.PingPong.SendPingCommand;

public class SendPingServicesConfiguration : IServicesConfiguration
{
    public void Configure(IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton<PongService>();
    }
}
