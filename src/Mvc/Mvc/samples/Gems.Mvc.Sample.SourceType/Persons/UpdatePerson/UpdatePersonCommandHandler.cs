// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

using Gems.Mvc.GenericControllers;
using Gems.Mvc.Sample.SourceType.Persons.CreatePerson;

using MediatR;

namespace Gems.Mvc.Sample.SourceType.Persons.UpdatePerson;

[Endpoint("api/v1/persons", "PUT", OperationGroup = "Persons", SourceType = GenericControllers.SourceType.FromBody)]
public class UpdatePersonCommandHandler : IRequestHandler<CreatePersonCommand, Guid>
{
    public Task<Guid> Handle(CreatePersonCommand request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
