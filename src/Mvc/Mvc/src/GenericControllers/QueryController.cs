// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

using System.Threading;
using System.Threading.Tasks;

using MediatR;

using Microsoft.AspNetCore.Mvc;

namespace Gems.Mvc.GenericControllers
{
    public class QueryController<TRequest> where TRequest : class, IRequest
    {
        private readonly IMediator mediator;

        public QueryController(IMediator mediator)
        {
            this.mediator = mediator;
        }

        [HttpGet]
        public Task Get([FromQuery] TRequest request, CancellationToken cancellationToken)
        {
            return this.mediator.Send(request, cancellationToken);
        }
    }
}
