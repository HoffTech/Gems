// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

using System.Threading;
using System.Threading.Tasks;

using Gems.Jobs.Quartz.Handlers.Consts;
using Gems.Jobs.Quartz.Handlers.Shared;
using Gems.Mvc.GenericControllers;

using MediatR;

using Quartz;

namespace Gems.Jobs.Quartz.Handlers.ResetTriggerFromErrorState
{
    [Endpoint("jobs/{JobName}/reset-from-error-state", "PUT", OperationGroup = "jobs")]
    public class ResetTriggerFromErrorStateCommandHandler : IRequestHandler<ResetTriggerFromErrorStateCommand>
    {
        private readonly SchedulerProvider schedulerProvider;

        public ResetTriggerFromErrorStateCommandHandler(SchedulerProvider schedulerProvider)
        {
            this.schedulerProvider = schedulerProvider;
        }

        public async Task Handle(ResetTriggerFromErrorStateCommand command, CancellationToken cancellationToken)
        {
            if (command.JobGroup == "string")
            {
                command.JobGroup = null;
            }

            var scheduler = await this.schedulerProvider.GetSchedulerAsync(cancellationToken).ConfigureAwait(false);

            var triggerKey = new TriggerKey(command.JobName, command.JobGroup ?? JobGroups.DefaultGroup);

            var triggerState = await scheduler.GetTriggerState(triggerKey, cancellationToken).ConfigureAwait(false);
            if (triggerState is TriggerState.Error)
            {
                await scheduler.ResetTriggerFromErrorState(triggerKey, cancellationToken).ConfigureAwait(false);
            }
        }
    }
}
