// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

namespace Gems.Mvc.Sample.HandlersUsing.Persons.CreatePerson;

public class CreatePersonServicesConfiguration : IServicesConfiguration
{
    public void Configure(IServiceCollection services, IConfiguration configuration)
    {
        services.AddConverter<ValidationExceptionToBusinessException>();
    }
}
