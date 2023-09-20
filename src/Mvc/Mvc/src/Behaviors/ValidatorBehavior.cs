// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using FluentValidation;

using MediatR;

namespace Gems.Mvc.Behaviors
{
    public class ValidatorBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    {
        private readonly IEnumerable<IValidator<TRequest>> validators;
        private readonly IEnumerable<IConverter<ValidationException, Exception, TRequest>> customConverters;

        public ValidatorBehavior(IEnumerable<IValidator<TRequest>> validators, IEnumerable<IConverter<ValidationException, Exception, TRequest>> customConverters)
        {
            this.validators = validators;
            this.customConverters = customConverters;
        }

        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        {
            var context = new ValidationContext<TRequest>(request);
            var results = await Task.WhenAll(this.validators
                .Select(v => v.ValidateAsync(context, cancellationToken))).ConfigureAwait(false);
            if (results.All(x => x.IsValid))
            {
                return await next().ConfigureAwait(false);
            }

            var failures = results.SelectMany(r => r.Errors)
                .Where(f => f != null)
                .ToList();

            var validationException = new ValidationException(failures);

            var customConverter = this.customConverters.FirstOrDefault();
            if (customConverter == null)
            {
                throw validationException;
            }

            throw customConverter.Convert(validationException, request);
        }
    }
}
