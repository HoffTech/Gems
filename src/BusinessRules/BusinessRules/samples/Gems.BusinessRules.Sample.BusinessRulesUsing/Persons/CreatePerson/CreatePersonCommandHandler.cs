// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

using Gems.BusinessRules.Sample.BusinessRulesUsing.Persons.CreatePerson.BusinessRules;
using Gems.Mvc.GenericControllers;

using MediatR;

namespace Gems.BusinessRules.Sample.BusinessRulesUsing.Persons.CreatePerson;

[Endpoint("api/v1/persons", "POST", OperationGroup = "Persons")]
public class CreatePersonCommandHandler : IRequestHandler<CreatePersonCommand, Guid>
{
    private readonly BusinessRulesChecker businessRulesChecker;

    public CreatePersonCommandHandler(BusinessRulesChecker businessRulesChecker)
    {
        this.businessRulesChecker = businessRulesChecker;
    }

    public Task<Guid> Handle(CreatePersonCommand request, CancellationToken cancellationToken)
    {
        this.businessRulesChecker.CheckPersonAge(request.Age);

        return Task.FromResult(Guid.Empty);
    }
}
