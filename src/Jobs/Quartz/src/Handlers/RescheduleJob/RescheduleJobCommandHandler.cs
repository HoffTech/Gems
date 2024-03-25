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
using Gems.Mvc.Filters.Exceptions;
using Gems.Mvc.GenericControllers;

using MediatR;

using Microsoft.Extensions.Options;

using Quartz;
using Quartz.Impl.Triggers;

using InvalidOperationException = Gems.Mvc.Filters.Exceptions.InvalidOperationException;

namespace Gems.Jobs.Quartz.Handlers.RescheduleJob
{
    [Endpoint("jobs/{JobName}", "PUT", OperationGroup = "jobs")]
    public class RescheduleJobCommandHandler : IRequestHandler<RescheduleJobCommand>
    {
        private readonly SchedulerProvider schedulerProvider;
        private readonly IOptions<JobsOptions> jobsOptions;
        private readonly TriggerHelper triggerHelper;

        public RescheduleJobCommandHandler(
            SchedulerProvider schedulerProvider,
            IOptions<JobsOptions> jobsOptions,
            TriggerHelper triggerHelper)
        {
            this.schedulerProvider = schedulerProvider;
            this.jobsOptions = jobsOptions;
            this.triggerHelper = triggerHelper;
        }

        public async Task Handle(RescheduleJobCommand command, CancellationToken cancellationToken)
        {
            if (command.JobGroup == "string")
            {
                command.JobGroup = null;
            }

            var scheduler = await this.schedulerProvider.GetSchedulerAsync(cancellationToken).ConfigureAwait(false);
            if (await this.RescheduleTriggerWithData(scheduler, command.JobName, command.TriggerName, command.JobGroup, command.CronExpression, cancellationToken).ConfigureAwait(false))
            {
                return;
            }

            if (await this.RescheduleTriggerFromDb(scheduler, command.JobName, command.TriggerName, command.JobGroup, command.CronExpression, cancellationToken).ConfigureAwait(false))
            {
                return;
            }

            var trigger = await scheduler
                .GetTrigger(
                    new TriggerKey(command.JobName, command.JobGroup ?? JobGroups.DefaultGroup),
                    cancellationToken)
                .ConfigureAwait(false);

            if (trigger == null)
            {
                throw new NotFoundException($"Не найдено задание {command.JobGroup ?? JobGroups.DefaultGroup}.{command.JobName}");
            }

            var newTrigger = (CronTriggerImpl)trigger.Clone();
            newTrigger.CronExpression = new CronExpression(command.CronExpression);

            await scheduler.UnscheduleJob(trigger.Key, cancellationToken).ConfigureAwait(false);
            await scheduler.ScheduleJob(newTrigger, cancellationToken).ConfigureAwait(false);
        }

        private async Task<bool> RescheduleTriggerWithData(IScheduler scheduler, string jobName, string triggerName, string jobGroup, string cronExpression, CancellationToken cancellationToken)
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

            if (this.jobsOptions.Value.TriggersWithData.TryGetValue(jobName, out var triggersWithData)
                && triggersWithData.All(t => t.TriggerName != triggerName))
            {
                throw new InvalidOperationException($"Триггер с именем '{triggerName}' не был найден в конфигурации");
            }

            var triggerOptions = this.jobsOptions.Value.TriggersWithData.GetValueOrDefault(jobName).First(t => t.TriggerName == triggerName);
            var newTrigger = this.triggerHelper.CreateCronTrigger(
                triggerOptions.TriggerName ?? jobName,
                jobGroup ?? JobGroups.DefaultGroup,
                jobName,
                jobGroup ?? JobGroups.DefaultGroup,
                cronExpression ?? triggerOptions.CronExpression,
                triggerOptions.TriggerData);

            await scheduler.UnscheduleJob(new TriggerKey(newTrigger.Key.Name), cancellationToken).ConfigureAwait(false);
            await scheduler.ScheduleJob(newTrigger, cancellationToken).ConfigureAwait(false);
            return true;
        }

        private async Task<bool> RescheduleTriggerFromDb(IScheduler scheduler, string jobName, string triggerName, string jobGroup, string cronExpression, CancellationToken cancellationToken)
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

            var triggerProviderType = this.triggerHelper.GetTriggerDbType(triggerOptions);
            var triggerCronExpr = await triggerProviderType.GetCronExpression(triggerOptions.TriggerName, cancellationToken).ConfigureAwait(false);
            var triggerData = await triggerProviderType.GetTriggerData(triggerOptions.TriggerName, cancellationToken).ConfigureAwait(false);
            var newTrigger = this.triggerHelper.CreateCronTrigger(
                triggerOptions.TriggerName ?? jobName,
                jobGroup ?? JobGroups.DefaultGroup,
                jobName,
                jobGroup ?? JobGroups.DefaultGroup,
                cronExpression ?? triggerCronExpr,
                triggerData);

            await scheduler.UnscheduleJob(new TriggerKey(newTrigger.Key.Name), cancellationToken).ConfigureAwait(false);
            await scheduler.ScheduleJob(newTrigger, cancellationToken).ConfigureAwait(false);
            return true;
        }
    }
}
