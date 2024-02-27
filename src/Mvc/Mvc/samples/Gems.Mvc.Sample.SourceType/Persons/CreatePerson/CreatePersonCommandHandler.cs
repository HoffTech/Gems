// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

using Gems.Mvc.GenericControllers;

using MediatR;

namespace Gems.Mvc.Sample.SourceType.Persons.CreatePerson;

[Endpoint("api/v1/persons", "POST", OperationGroup = "Persons", SourceType = GenericControllers.SourceType.FromQuery)]
public class CreatePersonCommandHandler : IRequestHandler<CreatePersonCommand, Guid>
{
    public Task<Guid> Handle(CreatePersonCommand request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
