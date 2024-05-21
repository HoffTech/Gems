// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

using System;
using System.Threading.Tasks;

using Gems.Text.Json;

using MediatR;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

using Quartz;

namespace Gems.Jobs.Quartz.Jobs.JobWithData;

public abstract class QuartzJobWithDataBase<TCommand> : IJob where TCommand : class, IRequest, new()
{
    private readonly IMediator mediator;
    private readonly ILogger<QuartzJobWithDataBase<TCommand>> logger;
    private readonly IConfiguration configuration;

    protected QuartzJobWithDataBase(IMediator mediator, ILogger<QuartzJobWithDataBase<TCommand>> logger, IConfiguration configuration)
    {
        this.mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        this.configuration = configuration;
    }

    public async Task Execute(IJobExecutionContext context)
    {
        this.logger.LogInformation($"{typeof(TCommand).Name} {context.FireInstanceId} executing at {DateTime.UtcNow}");
        try
        {
            var command = this.GetCommandFromJobData(context);
            await this.mediator.Send(command, context.CancellationToken).ConfigureAwait(false);
        }
        finally
        {
            this.logger.LogInformation($"{typeof(TCommand).Name} {context.FireInstanceId} executed at {DateTime.UtcNow}");
        }
    }

    private TCommand GetCommandFromJobData(IJobExecutionContext context)
    {
        try
        {
            var jobDataValue = context.MergedJobDataMap.GetString(QuartzJobWithDataConstants.JobDataKeyValue);
            var command = jobDataValue?.Deserialize<TCommand>();
            if (command != null)
            {
                return command;
            }

            var triggerName = context.MergedJobDataMap.GetString("TriggerName");
            if (triggerName == null)
            {
                return new TCommand();
            }

            var triggerPosition = context.MergedJobDataMap.GetString("TriggerPosition");
            if (triggerPosition == null)
            {
                return new TCommand();
            }

            command = this.configuration.GetSection($"Jobs:TriggersWithData:{context.Trigger.JobKey.Name}:{triggerPosition}:TriggerData").Get<TCommand>();
            if (command != null)
            {
                return command;
            }

            return new TCommand();
        }
        catch (Exception e)
        {
            this.logger.LogError(e, $"{context.FireInstanceId} JobData for {typeof(TCommand).Name} can't be deserialized");
            throw;
        }
    }
}
