// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Gems.Jobs.Quartz.Configuration;
using Gems.Jobs.Quartz.Consts;
using Gems.Jobs.Quartz.Handlers.Consts;
using Gems.Jobs.Quartz.Handlers.Shared;
using Gems.Jobs.Quartz.Jobs.JobWithData;
using Gems.Mvc.GenericControllers;
using Gems.Text.Json;

using MediatR;

using Microsoft.Extensions.Options;

using Quartz;
using Quartz.Impl.Triggers;

namespace Gems.Jobs.Quartz.Handlers.ScheduleJob
{
    [Endpoint("jobs/{JobName}", "POST", OperationGroup = "jobs")]
    public class ScheduleJobCommandHandler : IRequestHandler<ScheduleJobCommand>
    {
        private readonly IOptions<JobsOptions> options;
        private readonly SchedulerProvider schedulerProvider;

        public ScheduleJobCommandHandler(IOptions<JobsOptions> options, SchedulerProvider schedulerProvider)
        {
            this.options = options;
            this.schedulerProvider = schedulerProvider;
        }

        public async Task Handle(ScheduleJobCommand request, CancellationToken cancellationToken)
        {
            var scheduler = await this.schedulerProvider.GetSchedulerAsync(cancellationToken).ConfigureAwait(false);
            var trigger = await scheduler
                .GetTrigger(
                    new TriggerKey(request.JobName, request.JobGroup ?? JobGroups.DefaultGroup),
                    cancellationToken)
                .ConfigureAwait(false);

            if (trigger != null)
            {
                throw new InvalidOperationException($"Такое задание уже зарегистрировано {request.JobGroup ?? JobGroups.DefaultGroup}.{request.JobName}");
            }

            string cronExpression = null;
            if (this.options.Value.TriggersOptions != null)
            {
                foreach (var triggerOption in this.options.Value.TriggersOptions.Where(triggerOption => this.options.Value.Triggers.ContainsKey(triggerOption.Key)))
                {
                    this.options.Value.Triggers[triggerOption.Key] = triggerOption.Value;
                }
            }

            var triggerDataOptions = this.options.Value.Triggers
                .Where(r => r.Key == request.JobName)
                .Select(r => r.Value)
                .First();
            var triggersData = await TriggerRegister.GetTriggerData(triggerDataOptions).ConfigureAwait(false);
            foreach (var triggerOptions in triggersData)
            {
                if (!string.IsNullOrEmpty(request.CronExpression))
                {
                    cronExpression = request.CronExpression;
                }

                if (!string.IsNullOrEmpty(triggerOptions.CronExpression))
                {
                    cronExpression = triggerOptions.CronExpression;
                }

                var newTrigger = new CronTriggerImpl(
                    request.JobName,
                    request.JobGroup ?? JobGroups.DefaultGroup,
                    request.JobName,
                    request.JobGroup ?? JobGroups.DefaultGroup,
                    cronExpression);

                if (triggerOptions.TriggerData != null)
                {
                    newTrigger.JobDataMap = new JobDataMap { [QuartzJobWithDataConstants.JobDataKeyValue] = triggerOptions.TriggerData.Serialize() };
                }

                await scheduler.ScheduleJob(newTrigger, cancellationToken).ConfigureAwait(false);
            }
        }
    }
}
