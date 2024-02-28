// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

using Gems.Mvc.Behaviors;
using Gems.Mvc.NotFound.Persons.GetPerson.Dto;

using MediatR;

using Microsoft.AspNetCore.Mvc;

namespace Gems.Mvc.NotFound.Persons.GetPerson;

public class GetPersonQuery : IRequest<Person>, IRequestNotFound
{
    [FromRoute]
    public int PersonId { get; set; }

    public string GetNotFoundErrorMessage()
    {
        return $"Не найден пользователь с Id: {this.PersonId}";
    }
}
