// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

using Gems.Mvc.GenericControllers;

using MediatR;

namespace Gems.Mvc.Sample.LoadFile.Persons.ImportPerson;

[Endpoint("api/v1/persons/import", "POST", OperationGroup = "Persons", IsForm = true)]
public class ImportPersonCommandHandler : IRequestHandler<ImportPersonCommand>
{
    public Task Handle(ImportPersonCommand command, CancellationToken cancellationToken)
    {
        // Что-то делаем с command.CsvFile...
        throw new NotImplementedException();
    }
}
