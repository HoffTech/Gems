// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

using Gems.Data.Behaviors;

using MediatR;

namespace Gems.Mvc.RequestException.Persons.ImportPersons;

public class ImportPersonsCommand : IRequest, IRequestUnitOfWork
{
    public IFormFile CsvFile { get; set; }
}
