// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

using Microsoft.Extensions.Options;

namespace Gems.Http.Samples.UseTemplateUri.PingPong.SendPingCommand.PongApi;

public class PongService(
    IConfiguration configuration,
    IOptions<HttpClientServiceOptions> options,
    BaseClientServiceHelper helper)
    : BaseClientService<string>(options, helper)
{
    protected override string BaseUrl => configuration?.GetConnectionString("PongApiUrl") ?? throw new InvalidOperationException();

    public Task<string> GetPong(CancellationToken cancellationToken)
    {
        return this.GetAsync<string>("v1/samples/{secret}/pong".ToTemplateUri("ping"), cancellationToken);
    }
}
