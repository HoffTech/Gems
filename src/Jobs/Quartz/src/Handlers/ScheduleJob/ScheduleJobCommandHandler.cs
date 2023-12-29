// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Gems.Jobs.Quartz.Configuration;
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

            if (this.options.Value.Triggers.ContainsKey(request.JobName))
            {
                var cronExpression = this.options.Value.Triggers
                    .Where(r => r.Key == request.JobName)
                    .Select(r => r.Value)
                    .First();

                var newTrigger = CreateCronTrigger(
                    request.JobName,
                    request.JobGroup ?? JobGroups.DefaultGroup,
                    request.JobName,
                    request.JobGroup ?? JobGroups.DefaultGroup,
                    cronExpression ?? request.CronExpression,
                    null);

                await scheduler.ScheduleJob(newTrigger, cancellationToken).ConfigureAwait(false);
                return;
            }

            if (this.options.Value.TriggersWithData.ContainsKey(request.JobName))
            {
                foreach (var newTrigger in this.options.Value.TriggersWithData.GetValueOrDefault(request.JobName)
                             .Select(
                                 triggerWithData => CreateCronTrigger(
                                     triggerWithData.TriggerName ?? request.JobName,
                                     request.JobGroup ?? JobGroups.DefaultGroup,
                                     request.JobName,
                                     request.JobGroup ?? JobGroups.DefaultGroup,
                                     triggerWithData.CronExpression ?? request.CronExpression,
                                     triggerWithData.TriggerData)))
                {
                    await scheduler.ScheduleJob(newTrigger, cancellationToken).ConfigureAwait(false);
                }

                return;
            }

            if (this.options.Value.TriggersFromDb.ContainsKey(request.JobName))
            {
                foreach (var triggerFromDb in this.options.Value.TriggersFromDb.GetValueOrDefault(request.JobName).Where(t => Type.GetType(t.ProviderType)?.GetInterface(nameof(ITriggerDataProvider)) != null))
                {
                    var cronExpression = await (Type.GetType(triggerFromDb.ProviderType) as ITriggerDataProvider).GetCronExpression(triggerFromDb.TriggerName, cancellationToken).ConfigureAwait(false);
                    var triggerDataDict = await (Type.GetType(triggerFromDb.ProviderType) as ITriggerDataProvider).GetTriggerData(triggerFromDb.TriggerName, cancellationToken).ConfigureAwait(false);
                    var newTrigger = CreateCronTrigger(
                        triggerFromDb.TriggerName ?? request.JobName,
                        request.JobGroup ?? JobGroups.DefaultGroup,
                        request.JobName,
                        request.JobGroup ?? JobGroups.DefaultGroup,
                        cronExpression ?? request.CronExpression,
                        triggerDataDict);

                    await scheduler.ScheduleJob(newTrigger, cancellationToken).ConfigureAwait(false);
                }
            }
        }

        private static CronTriggerImpl CreateCronTrigger(
            string triggerName,
            string triggerGroup,
            string jobName,
            string jobGroup,
            string cronExp,
            Dictionary<string, object> triggerData)
        {
            var newTrigger = new CronTriggerImpl(triggerName, triggerGroup, jobName, jobGroup, cronExp);
            if (triggerData != null && triggerData.Any())
            {
                newTrigger.JobDataMap = new JobDataMap { [QuartzJobWithDataConstants.JobDataKeyValue] = triggerData.Serialize() };
            }

            return newTrigger;
        }
    }
}
