using System.Threading;
using System.Threading.Tasks;

using MediatR;

namespace Gems.Jobs.Quartz;

public interface IQuartzEnqueueManager
{
    Task Enqueue<TCommand>(TCommand command, CancellationToken cancellationToken) where TCommand : class, IRequest, new();
}
