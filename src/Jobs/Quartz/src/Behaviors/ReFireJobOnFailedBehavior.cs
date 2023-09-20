// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

using System;
using System.Threading;
using System.Threading.Tasks;

using MediatR;

using Quartz;

namespace Gems.Jobs.Quartz.Behaviors
{
    public class ReFireJobOnFailedBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
        where TRequest : IRequestReFireJobOnFailed
    {
        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        {
            try
            {
                return await next().ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                await Task.Delay(request.GetReFireJobOnErrorDelay(), cancellationToken).ConfigureAwait(false);

                var jobException = new JobExecutionException(ex)
                {
                    RefireImmediately = true
                };
                throw jobException;
            }
        }
    }
}
