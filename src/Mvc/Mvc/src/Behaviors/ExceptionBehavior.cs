// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

using FluentValidation;

using Gems.Mvc.Filters.Exceptions;

using MediatR;

using Microsoft.Extensions.Logging;

using Npgsql;

using InvalidOperationException = System.InvalidOperationException;

namespace Gems.Mvc.Behaviors
{
    public class ExceptionBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    {
        private readonly ILogger<ExceptionBehavior<TRequest, TResponse>> logger;

        public ExceptionBehavior(ILogger<ExceptionBehavior<TRequest, TResponse>> logger)
        {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        {
            Func<Exception, bool> needThrowException = _ => true;
            if (request is IRequestException ext)
            {
                needThrowException = ext.NeedThrowException;
            }

            try
            {
                var response = await next().ConfigureAwait(false);
                return response;
            }
            catch (NpgsqlException ex)
            {
                var detail = (ex as PostgresException)?.Detail ?? ex.Message;
                this.logger.LogError(ex, detail);
                if (needThrowException(ex))
                {
                    throw;
                }
            }
            catch (Exception ex) when (ex is BusinessException or ValidationException or InvalidDataException or InvalidOperationException)
            {
                this.logger.LogWarning(ex, ex.Message);
                if (needThrowException(ex))
                {
                    throw;
                }
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, ex.Message);
                if (needThrowException(ex))
                {
                    throw;
                }
            }

            return default;
        }
    }
}
