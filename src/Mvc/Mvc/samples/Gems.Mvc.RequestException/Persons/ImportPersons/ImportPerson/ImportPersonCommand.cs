// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

using Gems.Data.Behaviors;
using Gems.Mvc.Behaviors;
using Gems.Mvc.Filters.Exceptions;
using Gems.Mvc.RequestException.Persons.ImportPersons.Entities;

using MediatR;

namespace Gems.Mvc.RequestException.Persons.ImportPersons.ImportPerson;

public class ImportPersonCommand : IRequest, IRequestUnitOfWork, IRequestException
{
    public Person Person { get; set; }

    public bool NeedThrowException(Exception exception)
    {
        return exception is BusinessException { StatusCode: >= 499 };
    }
}
