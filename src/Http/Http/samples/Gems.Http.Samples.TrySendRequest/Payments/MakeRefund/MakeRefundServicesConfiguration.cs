// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

using Gems.Http.Samples.TrySendRequest.Payments.MakeRefund.PaymentApi;
using Gems.Http.Samples.TrySendRequest.Payments.MakeRefund.PaymentApi.Options;
using Gems.Mvc;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Gems.Http.Samples.TrySendRequest.Payments.MakeRefund
{
    public class MakeRefundServicesConfiguration : IServicesConfiguration
    {
        public void Configure(IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<PaymentApiOptions>(configuration.GetSection(PaymentApiOptions.Name));
            services.AddSingleton<PaymentService>();
        }
    }
}
