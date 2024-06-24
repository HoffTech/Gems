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
using Gems.Mvc.GenericControllers;

using MediatR;

using Microsoft.Extensions.Options;

using Quartz;

namespace Gems.Jobs.Quartz.Handlers.ScheduleJob
{
    [Endpoint("jobs/{JobName}", "POST", OperationGroup = "jobs")]
    public class ScheduleJobCommandHandler : IRequestHandler<ScheduleJobCommand>
    {
        private readonly IOptions<JobsOptions> options;
        private readonly SchedulerProvider schedulerProvider;
        private readonly TriggerHelper triggerHelper;
        private readonly StoredCronTriggerProvider storedCronTriggerProvider;

        public ScheduleJobCommandHandler(
            IOptions<JobsOptions> options,
            SchedulerProvider schedulerProvider,
            TriggerHelper triggerHelper,
            StoredCronTriggerProvider storedCronTriggerProvider)
        {
            this.options = options;
            this.schedulerProvider = schedulerProvider;
            this.triggerHelper = triggerHelper;
            this.storedCronTriggerProvider = storedCronTriggerProvider;
        }

        public async Task Handle(ScheduleJobCommand command, CancellationToken cancellationToken)
        {
            if (command.JobGroup == "string")
            {
                command.JobGroup = null;
            }

            var scheduler = await this.schedulerProvider.GetSchedulerAsync(cancellationToken).ConfigureAwait(false);
            var trigger = await scheduler
                .GetTrigger(
                    new TriggerKey(command.JobName, command.JobGroup ?? JobGroups.DefaultGroup),
                    cancellationToken)
                .ConfigureAwait(false);

            if (trigger != null)
            {
                throw new InvalidOperationException($"Такое задание уже зарегистрировано {command.JobGroup ?? JobGroups.DefaultGroup}.{command.JobName}");
            }

            if (await this.ScheduleSimpleTrigger(scheduler, command.JobName, command.JobGroup, command.CronExpression, cancellationToken).ConfigureAwait(false))
            {
                await this.WriteToPersistenceStore(command, cancellationToken);
                return;
            }

            if (await this.ScheduleTriggerWithData(scheduler, command.JobName, command.JobGroup, command.CronExpression, command.TriggerName, cancellationToken).ConfigureAwait(false))
            {
                await this.WriteToPersistenceStore(command, cancellationToken);
                return;
            }

            await this.ScheduleTriggerFromDb(scheduler, command.JobName, command.JobGroup, command.CronExpression, command.TriggerName, cancellationToken).ConfigureAwait(false);

            await this.WriteToPersistenceStore(command, cancellationToken);
        }

        private async Task WriteToPersistenceStore(ScheduleJobCommand command, CancellationToken cancellationToken)
        {
            if (!command.NeedWriteToPersistenceStore || !this.options.Value.EnablePersistenceStore)
            {
                return;
            }

            await this.storedCronTriggerProvider.WriteCronExpression(command.TriggerName, command.CronExpression, cancellationToken).ConfigureAwait(false);
        }

        private static async Task<List<string>> GetTriggersForSchedule(IScheduler scheduler, string jobName, List<string> triggersFromConfiguration, CancellationToken cancellationToken)
        {
            var jobTriggers = (await scheduler.GetTriggersOfJob(new JobKey(jobName), cancellationToken).ConfigureAwait(false)).ToList();
            return triggersFromConfiguration.Where(triggerName => !jobTriggers.Exists(t => t.Key.Name == triggerName)).ToList();
        }

        private async Task<bool> ScheduleSimpleTrigger(IScheduler scheduler, string jobName, string jobGroup, string cronExpression, CancellationToken cancellationToken)
        {
            if (this.options.Value.Triggers == null || !this.options.Value.Triggers.ContainsKey(jobName))
            {
                return false;
            }

            var triggerCronExpression = this.options.Value.Triggers
                .Where(r => r.Key == jobName)
                .Select(r => r.Value)
                .First();

            var newTrigger = TriggerHelper.CreateCronTrigger(
                jobName,
                jobGroup ?? JobGroups.DefaultGroup,
                jobName,
                jobGroup ?? JobGroups.DefaultGroup,
                cronExpression ?? triggerCronExpression,
                null);

            await scheduler.ScheduleJob(newTrigger, cancellationToken).ConfigureAwait(false);
            return true;
        }

        private async Task<bool> ScheduleTriggerWithData(
            IScheduler scheduler,
            string jobName,
            string jobGroup,
            string cronExpression,
            string triggerName,
            CancellationToken cancellationToken)
        {
            if (this.options.Value.TriggersWithData == null || !this.options.Value.TriggersWithData.ContainsKey(jobName))
            {
                return false;
            }

            if (string.IsNullOrEmpty(triggerName))
            {
                return await this.ScheduleTriggersWithData(scheduler, jobName, jobGroup, cronExpression, cancellationToken).ConfigureAwait(false);
            }

            var triggerFromConf = this.options.Value.TriggersWithData
                .GetValueOrDefault(jobName)
                .ToList()
                .First(t => t.TriggerName == triggerName);

            var trigger = TriggerHelper.CreateCronTrigger(
                triggerFromConf.TriggerName ?? jobName,
                jobGroup ?? JobGroups.DefaultGroup,
                jobName,
                jobGroup ?? JobGroups.DefaultGroup,
                cronExpression ?? triggerFromConf.CronExpression,
                triggerFromConf.TriggerData);
            await scheduler.ScheduleJob(trigger, cancellationToken).ConfigureAwait(false);
            return true;
        }

        private async Task<bool> ScheduleTriggersWithData(IScheduler scheduler, string jobName, string jobGroup, string cronExpression, CancellationToken cancellationToken)
        {
            var triggersFromConfiguration = new List<string>();
            foreach (var triggerWithData in this.options.Value.TriggersWithData.Where(t => t.Key == jobName).Select(t => t.Value))
            {
                triggersFromConfiguration.AddRange(triggerWithData.Select(t => t.TriggerName));
            }

            var triggersForSchedule = await GetTriggersForSchedule(scheduler, jobName, triggersFromConfiguration, cancellationToken).ConfigureAwait(false);

            foreach (var triggerNameForSchedule in triggersForSchedule)
            {
                await this.ScheduleTriggerWithData(scheduler, jobName, jobGroup, cronExpression, triggerNameForSchedule, cancellationToken).ConfigureAwait(false);
            }

            return true;
        }

        private async Task ScheduleTriggerFromDb(
            IScheduler scheduler,
            string jobName,
            string jobGroup,
            string cronExpression,
            string triggerName,
            CancellationToken cancellationToken)
        {
            if (this.options.Value.TriggersFromDb == null || !this.options.Value.TriggersFromDb.ContainsKey(jobName))
            {
                return;
            }

            if (string.IsNullOrEmpty(triggerName))
            {
                await this.ScheduleTriggersFromDb(scheduler, jobName, jobGroup, cronExpression, cancellationToken).ConfigureAwait(false);
                return;
            }

            var triggerFromDb = this.options.Value.TriggersFromDb
                .GetValueOrDefault(jobName)
                .ToList()
                .First(t => t.TriggerName == triggerName);

            var trigger = await this.triggerHelper.GetTriggerFromDb(jobName, jobGroup, cronExpression, triggerFromDb, cancellationToken);

            await scheduler.ScheduleJob(trigger, cancellationToken).ConfigureAwait(false);
        }

        private async Task ScheduleTriggersFromDb(IScheduler scheduler, string jobName, string jobGroup, string cronExpression, CancellationToken cancellationToken)
        {
            var triggersFromConfiguration = new List<string>();
            foreach (var triggerWithData in this.options.Value.TriggersFromDb.Where(t => t.Key == jobName).Select(t => t.Value))
            {
                triggersFromConfiguration.AddRange(triggerWithData.Select(t => t.TriggerName));
            }

            var triggersForSchedule = await GetTriggersForSchedule(scheduler, jobName, triggersFromConfiguration, cancellationToken).ConfigureAwait(false);

            foreach (var triggerNameForSchedule in triggersForSchedule)
            {
                await this.ScheduleTriggerFromDb(scheduler, jobName, jobGroup, cronExpression, triggerNameForSchedule, cancellationToken).ConfigureAwait(false);
            }
        }
    }
}
