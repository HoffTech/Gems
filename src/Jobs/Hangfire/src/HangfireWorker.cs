// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

using System;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;

using Hangfire;

using MediatR;

namespace Gems.Jobs.Hangfire
{
    public class HangfireWorker
    {
        private readonly IMediator mediator;

        public HangfireWorker(IMediator mediator)
        {
            this.mediator = mediator;
        }

        [DisableConcurrentExecution(60)]
        [AutomaticRetry(Attempts = 0)]
        [DisplayName("{0}")]
        public Task Run<T>(string name, T command, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentException($"'{nameof(name)}' cannot be null or empty.", nameof(name));
            }

            return this.mediator.Send(command, cancellationToken);
        }
    }
}
