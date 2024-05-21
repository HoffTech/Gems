// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

using Gems.Http.Samples.Logging.Security.Payments.CreatePayment.BankApi;
using Gems.Http.Samples.Logging.Security.Payments.CreatePayment.BankApi.Options;
using Gems.Mvc;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Gems.Http.Samples.Logging.Security.Payments.CreatePayment
{
    public class CreatePaymentServicesConfiguration : IServicesConfiguration
    {
        public void Configure(IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<BankApiOptions>(configuration.GetSection(BankApiOptions.Name));
            services.AddSingleton<BankService>();
        }
    }
}
