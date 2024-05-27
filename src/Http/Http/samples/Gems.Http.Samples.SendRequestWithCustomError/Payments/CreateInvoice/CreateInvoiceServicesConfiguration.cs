// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

using Gems.Http.Samples.SendRequestWithCustomError.Payments.CreateInvoice.PaymentApi;
using Gems.Http.Samples.SendRequestWithCustomError.Payments.CreateInvoice.PaymentApi.Options;
using Gems.Mvc;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Gems.Http.Samples.SendRequestWithCustomError.Payments.CreateInvoice
{
    public class CreateInvoiceServicesConfiguration : IServicesConfiguration
    {
        public void Configure(IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<PaymentApiOptions>(configuration.GetSection(PaymentApiOptions.Name));
            services.AddSingleton<PaymentService>();
        }
    }
}
