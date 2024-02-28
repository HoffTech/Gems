// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

using FluentValidation;

using Gems.Mvc.Filters.Exceptions;

namespace Gems.Mvc.Sample.HandlersUsing.Persons.CreatePerson;

public class ValidationExceptionToBusinessException : IValidationExceptionConverter<CreatePersonCommand>
{
    public Exception Convert(ValidationException exception, CreatePersonCommand command)
    {
        return new BusinessException("Некорректный запрос к сервису")
        {
            Error = { IsBusiness = true },
            StatusCode = 400
        };
    }
}
