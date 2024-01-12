// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Gems.Jobs.Quartz.Configuration;
using Gems.Jobs.Quartz.Handlers.Consts;
using Gems.Jobs.Quartz.Handlers.Shared;
using Gems.Mvc.Filters.Exceptions;
using Gems.Mvc.GenericControllers;

using MediatR;

using Microsoft.Extensions.Options;

using Quartz;

namespace Gems.Jobs.Quartz.Handlers.RemoveJob
{
    [Endpoint("jobs/{JobName}", "DELETE", OperationGroup = "jobs")]
    public class RemoveJobCommandHandler : IRequestHandler<RemoveJobCommand>
    {
        private readonly SchedulerProvider schedulerProvider;
        private readonly IOptions<JobsOptions> jobsOptions;

        public RemoveJobCommandHandler(SchedulerProvider schedulerProvider, IOptions<JobsOptions> jobsOptions)
        {
            this.schedulerProvider = schedulerProvider;
            this.jobsOptions = jobsOptions;
        }

        public async Task Handle(RemoveJobCommand request, CancellationToken cancellationToken)
        {
            var scheduler = await this.schedulerProvider.GetSchedulerAsync(cancellationToken).ConfigureAwait(false);
            if (await this.RemoveTriggerWithData(scheduler, request.JobName, request.TriggerName, request.JobGroup, cancellationToken).ConfigureAwait(false))
            {
                return;
            }

            if (await this.RemoveTriggerFromDb(scheduler, request.JobName, request.TriggerName, request.JobGroup, cancellationToken).ConfigureAwait(false))
            {
                return;
            }

            await scheduler
                .UnscheduleJob(
                    new TriggerKey(request.JobName, request.JobGroup ?? JobGroups.DefaultGroup),
                    cancellationToken)
                .ConfigureAwait(false);
        }

        private async Task<bool> RemoveTriggerWithData(IScheduler scheduler, string jobName, string triggerName, string jobGroup, CancellationToken cancellationToken)
        {
            if (this.jobsOptions.Value.TriggersWithData == null || !this.jobsOptions.Value.TriggersWithData.ContainsKey(jobName))
            {
                return false;
            }

            if (string.IsNullOrEmpty(triggerName))
            {
                var triggersName = new List<string>();
                foreach (var triggerWithData in this.jobsOptions.Value.TriggersWithData.Where(t => t.Key == jobName).Select(t => t.Value))
                {
                    triggersName.AddRange(triggerWithData.Select(t => t.TriggerName));
                }

                await this.UnscheduleJobs(scheduler, jobName, triggersName, cancellationToken).ConfigureAwait(false);
                return true;
            }

            if (this.jobsOptions.Value.TriggersWithData.TryGetValue(jobName, out var triggersWithData)
                && triggersWithData.All(t => t.TriggerName != triggerName))
            {
                throw new InvalidOperationException(
                    $"Триггер с именем '{triggerName}' не был найден в конфигурации. Доступные триггеры: {string.Join(", ", this.jobsOptions.Value.TriggersWithData.GetValueOrDefault(jobName)
                        .Select(t => t.TriggerName))}");
            }

            var triggerOptions = this.jobsOptions.Value.TriggersWithData.GetValueOrDefault(jobName).First(t => t.TriggerName == triggerName);
            var triggerKey = new TriggerKey(triggerOptions.TriggerName, jobGroup ?? JobGroups.DefaultGroup);
            await scheduler.UnscheduleJob(triggerKey, cancellationToken).ConfigureAwait(false);
            return true;
        }

        private async Task<bool> RemoveTriggerFromDb(IScheduler scheduler, string jobName, string triggerName, string jobGroup, CancellationToken cancellationToken)
        {
            if (this.jobsOptions.Value.TriggersFromDb == null || !this.jobsOptions.Value.TriggersFromDb.ContainsKey(jobName))
            {
                return false;
            }

            if (string.IsNullOrEmpty(triggerName))
            {
                var triggersName = new List<string>();
                foreach (var triggerWithData in this.jobsOptions.Value.TriggersFromDb.Where(t => t.Key == jobName).Select(t => t.Value))
                {
                    triggersName.AddRange(triggerWithData.Select(t => t.TriggerName));
                }

                await this.UnscheduleJobs(scheduler, jobName, triggersName, cancellationToken).ConfigureAwait(false);
                return true;
            }

            if (this.jobsOptions.Value.TriggersFromDb.TryGetValue(jobName, out var triggersWithData)
                && triggersWithData.All(t => t.TriggerName != triggerName))
            {
                throw new InvalidOperationException(
                    $"Триггер с именем '{triggerName}' не был найден в конфигурации. Доступные триггеры: {string.Join(", ", this.jobsOptions.Value.TriggersWithData.GetValueOrDefault(jobName)
                        .Select(t => t.TriggerName))}");
            }

            var triggerOptions = this.jobsOptions.Value.TriggersFromDb.GetValueOrDefault(jobName).First(t => t.TriggerName == triggerName);
            var triggerKey = new TriggerKey(triggerOptions.TriggerName, jobGroup ?? JobGroups.DefaultGroup);
            await scheduler.UnscheduleJob(triggerKey, cancellationToken).ConfigureAwait(false);
            return true;
        }

        private async Task UnscheduleJobs(IScheduler scheduler, string jobName, List<string> triggersName, CancellationToken cancellationToken)
        {
            var jobTriggersListToUnschedule = new List<ITrigger>();
            var jobKey = new JobKey(jobName);
            var jobTriggers = (await scheduler.GetTriggersOfJob(jobKey, cancellationToken).ConfigureAwait(false)).ToList();
            jobTriggersListToUnschedule.AddRange(jobTriggers.Where(trigger => triggersName.Contains(trigger.Key.Name)));
            await scheduler.UnscheduleJobs(jobTriggersListToUnschedule.Select(j => j.Key).ToList(), cancellationToken).ConfigureAwait(false);
        }
    }
}
