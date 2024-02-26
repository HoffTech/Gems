// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

using FluentValidation;

namespace Gems.Mvc.Sample.HandlersUsing.Persons.CreatePerson.Validation;

public class CreatePersonCommandValidator : AbstractValidator<CreatePersonCommand>
{
    public CreatePersonCommandValidator()
    {
        this.RuleFor(m => m.FirstName)
            .NotEmpty()
            .MaximumLength(20);

        this.RuleFor(m => m.LastName)
            .NotEmpty()
            .MaximumLength(20);

        this.RuleFor(m => m.Age)
            .GreaterThanOrEqualTo(0);
    }
}
