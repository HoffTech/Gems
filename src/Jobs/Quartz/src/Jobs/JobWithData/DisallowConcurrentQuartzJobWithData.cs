using MediatR;

using Microsoft.Extensions.Logging;

using Quartz;

namespace Gems.Jobs.Quartz.Jobs.JobWithData;

[DisallowConcurrentExecution]
public sealed class DisallowConcurrentQuartzJobWithData<TCommand> : QuartzJobWithDataBase<TCommand> where TCommand : class, IRequest, new()
{
    public DisallowConcurrentQuartzJobWithData(
        IMediator mediator,
        ILogger<DisallowConcurrentQuartzJobWithData<TCommand>> logger)
        : base(mediator, logger)
    {
    }
}
