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
using Gems.Mvc.Filters.Exceptions;
using Gems.Mvc.GenericControllers;

using MediatR;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using Quartz;

using InvalidOperationException = Gems.Mvc.Filters.Exceptions.InvalidOperationException;

namespace Gems.Jobs.Quartz.Handlers.RescheduleJob
{
    [Endpoint("jobs/{JobName}", "PUT", OperationGroup = "jobs")]
    public class RescheduleJobCommandHandler : IRequestHandler<RescheduleJobCommand>
    {
        private readonly SchedulerProvider schedulerProvider;
        private readonly IOptions<JobsOptions> jobsOptions;
        private readonly TriggerHelper triggerHelper;
        private readonly StoredCronTriggerProvider storedCronTriggerProvider;
        private readonly ILogger<RescheduleJobCommandHandler> logger;

        public RescheduleJobCommandHandler(
            SchedulerProvider schedulerProvider,
            IOptions<JobsOptions> jobsOptions,
            TriggerHelper triggerHelper,
            ILogger<RescheduleJobCommandHandler> logger,
            StoredCronTriggerProvider storedCronTriggerProvider)
        {
            this.schedulerProvider = schedulerProvider;
            this.jobsOptions = jobsOptions;
            this.triggerHelper = triggerHelper;
            this.logger = logger;
            this.storedCronTriggerProvider = storedCronTriggerProvider;
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
                await this.WriteToPersistenceStore(command, cancellationToken);
                return;
            }

            if (await this.RescheduleTriggerFromDb(scheduler, command.JobName, command.TriggerName, command.JobGroup, command.CronExpression, cancellationToken).ConfigureAwait(false))
            {
                await this.WriteToPersistenceStore(command, cancellationToken);
                return;
            }

            var trigger = await scheduler
                .GetTrigger(
                    new TriggerKey(command.JobName, command.JobGroup ?? JobGroups.DefaultGroup),
                    cancellationToken)
                .ConfigureAwait(false) ?? throw new NotFoundException($"Не найдено задание {command.JobGroup ?? JobGroups.DefaultGroup}.{command.JobName}");

            var newTrigger = trigger.GetTriggerBuilder()
                .WithCronSchedule(command.CronExpression)
                .StartNow()
                .Build();

            var rescheduledJobNextTime = await scheduler.RescheduleJob(trigger.Key, newTrigger, cancellationToken)
                .ConfigureAwait(false);
            if (rescheduledJobNextTime.HasValue)
            {
                this.logger.LogInformation(
                    "Trigger ({TriggerKey}) successfully re-scheduled, next time:{rescheduledJobNextTime}",
                    trigger.JobKey.Name,
                    rescheduledJobNextTime);
            }
            else
            {
                this.logger.LogInformation("Trigger ({TriggerKey}) failed to re-schedule", trigger.JobKey.Name);
            }

            await this.WriteToPersistenceStore(command, cancellationToken);
        }

        private async Task WriteToPersistenceStore(RescheduleJobCommand command, CancellationToken cancellationToken)
        {
            if (!command.NeedWriteToPersistenceStore || !this.jobsOptions.Value.EnablePersistenceStore)
            {
                return;
            }

            await this.storedCronTriggerProvider.WriteCronExpression(command.TriggerName, command.CronExpression, cancellationToken).ConfigureAwait(false);
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
            var newTrigger = TriggerHelper.CreateCronTrigger(
                triggerOptions.TriggerName ?? jobName,
                jobGroup ?? JobGroups.DefaultGroup,
                jobName,
                jobGroup ?? JobGroups.DefaultGroup,
                cronExpression ?? triggerOptions.CronExpression,
                triggerOptions.TriggerData);

            var rescheduledJobNextTime = await scheduler.RescheduleJob(newTrigger.Key, newTrigger, cancellationToken)
                .ConfigureAwait(false);
            if (rescheduledJobNextTime.HasValue)
            {
                this.logger.LogInformation(
                    "Trigger ({TriggerKey}) successfully re-scheduled, next time:{rescheduledJobNextTime}",
                    newTrigger.JobKey.Name,
                    rescheduledJobNextTime);
                return true;
            }
            else
            {
                this.logger.LogInformation("Trigger ({TriggerKey}) failed to re-schedule", newTrigger.JobKey.Name);
                return false;
            }
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

            var newTrigger = await this.triggerHelper.GetTriggerFromDb(jobName, jobGroup, cronExpression, triggerOptions, cancellationToken);

            var rescheduledJobNextTime = await scheduler.RescheduleJob(newTrigger.Key, newTrigger, cancellationToken)
                .ConfigureAwait(false);
            if (rescheduledJobNextTime.HasValue)
            {
                this.logger.LogInformation(
                    "Trigger ({TriggerKey}) successfully re-scheduled, next time:{rescheduledJobNextTime}",
                    newTrigger.JobKey.Name,
                    rescheduledJobNextTime);
                return true;
            }

            this.logger.LogInformation("Trigger ({TriggerKey}) failed to re-schedule", newTrigger.JobKey.Name);
            return false;
        }
    }
}
