// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

using Gems.Http.Samples.DownloadCertificate.Payments.GetPayment.PaymentApi;
using Gems.Http.Samples.DownloadCertificate.Payments.GetPayment.PaymentApi.Options;
using Gems.Mvc;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Gems.Http.Samples.DownloadCertificate.Payments.GetPayment
{
    public class GetPaymentServicesConfiguration : IServicesConfiguration
    {
        public void Configure(IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<PaymentApiOptions>(configuration.GetSection(PaymentApiOptions.Name));
            services.AddSingleton<PaymentService>();
        }
    }
}
