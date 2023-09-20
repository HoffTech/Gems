// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

using System.Threading;

using Hangfire;

namespace Gems.Jobs.Hangfire
{
    public class HangfireEnqueueManager : IHangfireEnqueueManager
    {
        public void Enqueue<T>(string name, T command)
        {
            BackgroundJob.Enqueue<HangfireWorker>(x => x.Run(name, command, CancellationToken.None));
        }
    }
}
