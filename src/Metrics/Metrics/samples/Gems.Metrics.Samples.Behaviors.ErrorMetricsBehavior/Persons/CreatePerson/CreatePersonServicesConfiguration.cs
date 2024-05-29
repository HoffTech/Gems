// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

using Gems.Metrics.Samples.Behaviors.ErrorMetricsBehavior.Persons.CreatePerson.BusinessRules;
using Gems.Mvc;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Gems.Metrics.Samples.Behaviors.ErrorMetricsBehavior.Persons.CreatePerson
{
    public class CreatePersonServicesConfiguration : IServicesConfiguration
    {
        public void Configure(IServiceCollection services, IConfiguration configuration)
        {
            services.AddSingleton<BusinessRuleChecker>();
            services.AddSingleton<OnlyAdultAgeBusinessRule>();
        }
    }
}
