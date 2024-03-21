// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

using System.Threading;
using System.Threading.Tasks;

using Microsoft.Extensions.Hosting;

namespace Gems.Settings.Gitlab
{
    public class GitlabConfigurationBackgroundUpdater : BackgroundService
    {
        private readonly GitlabConfigurationUpdaterSettings settings;
        private readonly GitlabConfigurationUpdater updater;

        public GitlabConfigurationBackgroundUpdater(
            GitlabConfigurationUpdaterSettings settings,
            GitlabConfigurationUpdater updater)
        {
            this.settings = settings;
            this.updater = updater;
        }

        protected override async Task ExecuteAsync(CancellationToken ct)
        {
            while (!ct.IsCancellationRequested)
            {
                await this.updater.UpdateConfiguration();
                await Task.Delay(this.settings.UpdateInterval, ct);
            }
        }
    }
}
