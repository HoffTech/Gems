// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

using Gems.Mvc.GenericControllers;
using Gems.Mvc.RequestException.Persons.ImportPersons.Entities;
using Gems.Mvc.RequestException.Persons.ImportPersons.ImportPerson;

using MediatR;

namespace Gems.Mvc.RequestException.Persons.ImportPersons;

[Endpoint("api/v1/persons/import", "POST", OperationGroup = "Persons", IsForm = true)]
public class ImportPersonsCommandHandler : IRequestHandler<ImportPersonsCommand>
{
    private readonly IMediator mediator;

    public ImportPersonsCommandHandler(IMediator mediator)
    {
        this.mediator = mediator;
    }

    public async Task Handle(ImportPersonsCommand command, CancellationToken cancellationToken)
    {
        var persons = this.LoadPersons(command.CsvFile);

        foreach (var person in persons)
        {
            await this.mediator.Send(
                new ImportPersonCommand { Person = person },
                cancellationToken);
        }
    }

    private IEnumerable<Person> LoadPersons(IFormFile csvFile)
    {
        return new[] { new Person() };
    }
}
