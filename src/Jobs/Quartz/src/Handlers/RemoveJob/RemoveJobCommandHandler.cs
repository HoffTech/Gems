// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

using System.Threading;
using System.Threading.Tasks;

using Gems.Jobs.Quartz.Handlers.Consts;
using Gems.Jobs.Quartz.Handlers.Shared;
using Gems.Mvc.GenericControllers;

using MediatR;

using Quartz;

namespace Gems.Jobs.Quartz.Handlers.RemoveJob
{
    [Endpoint("jobs/{JobName}", "DELETE", OperationGroup = "jobs")]
    public class RemoveJobCommandHandler : IRequestHandler<RemoveJobCommand>
    {
        private readonly SchedulerProvider schedulerProvider;

        public RemoveJobCommandHandler(SchedulerProvider schedulerProvider)
        {
            this.schedulerProvider = schedulerProvider;
        }

        public async Task Handle(RemoveJobCommand request, CancellationToken cancellationToken)
        {
            var scheduler = await this.schedulerProvider.GetSchedulerAsync(cancellationToken).ConfigureAwait(false);
            await scheduler
                .UnscheduleJob(
                    new TriggerKey(request.JobName, request.JobGroup ?? JobGroups.DefaultGroup),
                    cancellationToken)
                .ConfigureAwait(false);
        }
    }
}
