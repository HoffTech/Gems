// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

using Gems.Mvc;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Gems.Data.Sample.Context.Persons.UpdatePerson
{
    public class UpdatePersonServicesConfiguration : IServicesConfiguration
    {
        public void Configure(IServiceCollection services, IConfiguration configuration)
        {
            services.AddSingleton<SessionRepository>();
        }
    }
}
