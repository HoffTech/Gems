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
using Gems.Jobs.Quartz.Jobs.JobTriggerFromDb;
using Gems.Jobs.Quartz.Jobs.JobWithData;
using Gems.Mvc.GenericControllers;
using Gems.Text.Json;

using MediatR;

using Microsoft.Extensions.Options;

using Quartz;

using InvalidOperationException = Gems.Mvc.Filters.Exceptions.InvalidOperationException;

namespace Gems.Jobs.Quartz.Handlers.FireJobImmediately
{
    [Endpoint("jobs/{JobName}/fire-immediately", "POST", OperationGroup = "jobs")]
    public class FireJobImmediatelyCommandHandler : IRequestHandler<FireJobImmediatelyCommand>
    {
        private readonly SchedulerProvider schedulerProvider;
        private readonly IOptions<JobsOptions> jobsOptions;
        private readonly TriggerHelper triggerHelper;

        public FireJobImmediatelyCommandHandler(
            SchedulerProvider schedulerProvider,
            IOptions<JobsOptions> jobsOptions,
            TriggerHelper triggerHelper)
        {
            this.schedulerProvider = schedulerProvider;
            this.jobsOptions = jobsOptions;
            this.triggerHelper = triggerHelper;
        }

        public async Task Handle(FireJobImmediatelyCommand command, CancellationToken cancellationToken)
        {
            if (command.JobGroup == "string")
            {
                command.JobGroup = null;
            }

            var scheduler = await this.schedulerProvider.GetSchedulerAsync(cancellationToken).ConfigureAwait(false);

            if (command.TriggerData != null && command.TriggerData.Any())
            {
                var jobDataMap = new JobDataMap { { QuartzJobWithDataConstants.JobDataKeyValue, command.TriggerData.Serialize() } };

                await scheduler.TriggerJob(new JobKey(command.JobName, command.JobGroup ?? JobGroups.DefaultGroup), jobDataMap, cancellationToken).ConfigureAwait(false);
                return;
            }

            if (await this.FireTriggerWithData(scheduler, command.JobName, command.TriggerName, command.JobGroup, cancellationToken).ConfigureAwait(false))
            {
                return;
            }

            if (await this.FireTriggerFromDb(scheduler, command.JobName, command.TriggerName, command.JobGroup, cancellationToken).ConfigureAwait(false))
            {
                return;
            }

            await scheduler
                .TriggerJob(new JobKey(command.JobName, command.JobGroup ?? JobGroups.DefaultGroup), cancellationToken)
                .ConfigureAwait(false);
        }

        private async Task<bool> FireTriggerWithData(IScheduler scheduler, string jobName, string triggerName, string jobGroup, CancellationToken cancellationToken)
        {
            if (this.jobsOptions.Value.TriggersWithData == null || !this.jobsOptions.Value.TriggersWithData.ContainsKey(jobName))
            {
                return false;
            }

            if (string.IsNullOrEmpty(triggerName))
            {
                throw new InvalidOperationException(
                    $"Необходимо указать имя триггера из списка: {string.Join(", ", this.jobsOptions.Value.TriggersWithData.GetValueOrDefault(jobName)
                        .Select(t => t.TriggerName))}");
            }

            var jobDataMap = new JobDataMap();
            jobDataMap.Add("TriggerName", triggerName);
            if (this.jobsOptions.Value.TriggersWithData.TryGetValue(jobName, out var triggersWithData))
            {
                var triggerFound = false;
                for (var triggerPosition = 0; triggerPosition < triggersWithData.Count; triggerPosition++)
                {
                    if (triggersWithData[triggerPosition].TriggerName == triggerName)
                    {
                        jobDataMap.Add("TriggerPosition", triggerPosition.ToString());
                        triggerFound = true;
                        break;
                    }
                }

                if (!triggerFound)
                {
                    throw new InvalidOperationException($"Триггер с именем '{triggerName}' не был найден в конфигурации");
                }
            }

            await scheduler.TriggerJob(new JobKey(jobName!, jobGroup ?? JobGroups.DefaultGroup), jobDataMap, cancellationToken).ConfigureAwait(false);
            return true;
        }

        private async Task<bool> FireTriggerFromDb(IScheduler scheduler, string jobName, string triggerName, string jobGroup, CancellationToken cancellationToken)
        {
            if (this.jobsOptions.Value.TriggersFromDb == null || !this.jobsOptions.Value.TriggersFromDb.ContainsKey(jobName))
            {
                return false;
            }

            if (string.IsNullOrEmpty(triggerName))
            {
                throw new InvalidOperationException(
                    $"Необходимо указать имя триггера из списка: {string.Join(", ", this.jobsOptions.Value.TriggersFromDb.GetValueOrDefault(jobName)
                        .Select(t => t.TriggerName))}");
            }

            if (this.jobsOptions.Value.TriggersFromDb.TryGetValue(jobName, out var triggersWithData)
                && triggersWithData.All(t => t.TriggerName != triggerName))
            {
                throw new InvalidOperationException($"Триггер с именем '{triggerName}' не был найден в конфигурации");
            }

            var triggerOptions = this.jobsOptions.Value.TriggersFromDb.GetValueOrDefault(jobName)?.First(t => t.TriggerName == triggerName);
            if (triggerOptions?.ProviderType == null || Type.GetType(triggerOptions.ProviderType)?.GetInterface(nameof(ITriggerDataProvider)) != null)
            {
                throw new InvalidOperationException($"Для триггера {triggerName} не был найден ProviderType или ProviderType не реализует интерфейс ITriggerDataProvider");
            }

            var triggerProviderType = this.triggerHelper.GetTriggerDbType(triggerOptions.ProviderType) ?? throw new InvalidOperationException(
                    $"Для триггера {triggerOptions.TriggerName}, тип {triggerOptions.ProviderType} не был найден или не реализует интерфейс ITriggerDataProvider");

            var triggerDataDict = await triggerProviderType.GetTriggerData(triggerOptions.TriggerName, cancellationToken).ConfigureAwait(false);
            var jobDataMap = new JobDataMap();
            if (triggerDataDict != null && triggerDataDict.Any())
            {
                jobDataMap.Add(QuartzJobWithDataConstants.JobDataKeyValue, triggerDataDict.Serialize());
            }

            await scheduler.TriggerJob(new JobKey(jobName, jobGroup ?? JobGroups.DefaultGroup), jobDataMap, cancellationToken).ConfigureAwait(false);
            return true;
        }
    }
}
