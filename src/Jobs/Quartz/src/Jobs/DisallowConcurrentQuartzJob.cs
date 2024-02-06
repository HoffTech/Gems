// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

using System;
using System.Threading.Tasks;

using MediatR;

using Microsoft.Extensions.Logging;

using Quartz;

namespace Gems.Jobs.Quartz.Jobs
{
    [DisallowConcurrentExecution]
    public class DisallowConcurrentQuartzJob<T> : IJob where T : class, IRequest, new()
    {
        private readonly IMediator mediator;
        private readonly ILogger<DisallowConcurrentQuartzJob<T>> logger;

        public DisallowConcurrentQuartzJob(IMediator mediator, ILogger<DisallowConcurrentQuartzJob<T>> logger)
        {
            this.mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task Execute(IJobExecutionContext context)
        {
            this.logger.LogInformation(
                "NonConcurrent Job {JobName} {FireInstanceId} executing at {Time}",
                typeof(T).Name,
                context.FireInstanceId,
                DateTime.UtcNow);

            try
            {
                await this.mediator.Send(new T(), context.CancellationToken).ConfigureAwait(false);
            }
            catch
            {
                // ignored
            }
            finally
            {
                this.logger.LogInformation(
                    "NonConcurrent Job {JobName} {FireInstanceId} executed at {Time}",
                    typeof(T).Name,
                    context.FireInstanceId,
                    DateTime.UtcNow);
            }
        }
    }
}
