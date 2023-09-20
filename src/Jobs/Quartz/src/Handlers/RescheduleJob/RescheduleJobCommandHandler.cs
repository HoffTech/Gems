// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

using System.Threading;
using System.Threading.Tasks;

using Gems.Jobs.Quartz.Handlers.Consts;
using Gems.Jobs.Quartz.Handlers.Shared;
using Gems.Mvc.Filters.Exceptions;
using Gems.Mvc.GenericControllers;

using MediatR;

using Quartz;
using Quartz.Impl.Triggers;

namespace Gems.Jobs.Quartz.Handlers.RescheduleJob
{
    [Endpoint("jobs/{JobName}", "PUT", OperationGroup = "jobs")]
    public class RescheduleJobCommandHandler : IRequestHandler<RescheduleJobCommand>
    {
        private readonly SchedulerProvider schedulerProvider;

        public RescheduleJobCommandHandler(SchedulerProvider schedulerProvider)
        {
            this.schedulerProvider = schedulerProvider;
        }

        public async Task Handle(RescheduleJobCommand request, CancellationToken cancellationToken)
        {
            var scheduler = await this.schedulerProvider.GetSchedulerAsync(cancellationToken).ConfigureAwait(false);
            var trigger = await scheduler
                .GetTrigger(
                    new TriggerKey(request.JobName, request.JobGroup ?? JobGroups.DefaultGroup),
                    cancellationToken)
                .ConfigureAwait(false);

            if (trigger == null)
            {
                throw new NotFoundException($"Не найдено задание {request.JobGroup ?? JobGroups.DefaultGroup}.{request.JobName}");
            }

            var newTrigger = (CronTriggerImpl)trigger.Clone();
            newTrigger.CronExpression = new CronExpression(request.CronExpression);

            await scheduler.UnscheduleJob(trigger.Key, cancellationToken).ConfigureAwait(false);
            await scheduler.ScheduleJob(newTrigger, cancellationToken).ConfigureAwait(false);
        }
    }
}
