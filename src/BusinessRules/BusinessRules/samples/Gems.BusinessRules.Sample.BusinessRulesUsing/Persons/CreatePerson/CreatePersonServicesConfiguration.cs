// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

using Gems.BusinessRules.Sample.BusinessRulesUsing.Persons.CreatePerson.BusinessRules;
using Gems.Mvc;

namespace Gems.BusinessRules.Sample.BusinessRulesUsing.Persons.CreatePerson;

public class CreatePersonServicesConfiguration : IServicesConfiguration
{
    public void Configure(IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton<PersonAgeBusinessRule>();
        services.AddSingleton<BusinessRulesChecker>();
    }
}
