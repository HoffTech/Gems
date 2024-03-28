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
