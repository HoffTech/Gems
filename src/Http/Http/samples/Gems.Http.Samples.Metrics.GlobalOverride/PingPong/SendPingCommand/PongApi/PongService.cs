// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

using System.Threading;
using System.Threading.Tasks;

using Gems.Mvc.Filters.Exceptions;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

namespace Gems.Http.Samples.Metrics.GlobalOverride.PingPong.SendPingCommand.PongApi;

public class PongService(
    IConfiguration configuration,
    IOptions<HttpClientServiceOptions> options,
    BaseClientServiceHelper helper)
    : BaseClientService<string>(options, helper)
{
    protected override string BaseUrl => configuration?.GetConnectionString("PongApiUrl") ?? throw new InvalidOperationException();

    public Task<string> GetPong(CancellationToken cancellationToken)
    {
        return this.GetAsync<string>("api/v1/samples/{secret}/pong".ToTemplateUri("ping"), cancellationToken);
    }
}
