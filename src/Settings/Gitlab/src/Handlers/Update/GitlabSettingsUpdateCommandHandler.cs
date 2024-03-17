// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

using System.Threading;
using System.Threading.Tasks;

using Gems.Mvc.GenericControllers;

using MediatR;

namespace Gems.Settings.Gitlab.Handlers.Update;

[Endpoint("gitlabsettings/update", "POST", OperationGroup = "gitlabsettings")]
public class GitlabSettingsUpdateCommandHandler : IRequestHandler<GitlabSettingsUpdateCommand>
{
    private readonly GitlabConfigurationUpdater updater;

    public GitlabSettingsUpdateCommandHandler(GitlabConfigurationUpdater updater)
    {
        this.updater = updater;
    }

    public async Task Handle(GitlabSettingsUpdateCommand request, CancellationToken cancellationToken)
    {
        await this.updater.UpdateConfiguration();
    }
}
