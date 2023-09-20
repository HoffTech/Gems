// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

using System.Threading;
using System.Threading.Tasks;

using MediatR;

using Microsoft.AspNetCore.Mvc;

namespace Gems.Mvc.GenericControllers
{
    public class PutBodySourceCommandWithResponseController<TRequest, TResponse> where TRequest : class, IRequest<TResponse>
    {
        private readonly IMediator mediator;

        public PutBodySourceCommandWithResponseController(IMediator mediator)
        {
            this.mediator = mediator;
        }

        [HttpPut]
        public async Task<TResponse> Put([FromBody] TRequest request, CancellationToken cancellationToken)
        {
            return await this.mediator.Send(request, cancellationToken).ConfigureAwait(false);
        }
    }
}
