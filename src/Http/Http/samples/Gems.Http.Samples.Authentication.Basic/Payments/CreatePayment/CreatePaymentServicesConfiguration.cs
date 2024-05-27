// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

using Gems.Http.Samples.Authentication.Basic.Payments.CreatePayment.BankApi;
using Gems.Http.Samples.Authentication.Basic.Payments.CreatePayment.BankApi.Options;
using Gems.Mvc;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Gems.Http.Samples.Authentication.Basic.Payments.CreatePayment
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
