// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

using FluentValidation;

namespace Gems.Metrics.Samples.Behaviors.ErrorMetricsBehavior.Persons.CreatePerson.Validation
{
    public class CreatePersonCommandValidator : AbstractValidator<CreatePersonCommand>
    {
        public CreatePersonCommandValidator()
        {
            this.RuleFor(p => p.FirstName).NotEmpty();
            this.RuleFor(p => p.LastName).NotEmpty();
            this.RuleFor(p => p.Age).NotEmpty().GreaterThan(0);
        }
    }
}
