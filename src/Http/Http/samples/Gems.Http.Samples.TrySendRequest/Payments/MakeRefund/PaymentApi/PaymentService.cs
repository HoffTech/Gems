// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

using System.Threading;
using System.Threading.Tasks;

using Gems.Http.Samples.TrySendRequest.Payments.MakeRefund.PaymentApi.Dto;
using Gems.Http.Samples.TrySendRequest.Payments.MakeRefund.PaymentApi.Options;
using Gems.Mvc.Filters.Errors;

using Microsoft.Extensions.Options;

namespace Gems.Http.Samples.TrySendRequest.Payments.MakeRefund.PaymentApi
{
    public class PaymentService(IOptions<PaymentApiOptions> options, BaseClientServiceHelper helper)
        : BaseClientService<BusinessErrorViewModel>(options, helper)
    {
        public Task<(Unit, BusinessErrorViewModel)> RefundAsync(
            RefundRequest refundRequest,
            CancellationToken cancellationToken)
        {
            return this
                .TryPostAsync(
                    "api/v1/payment/refund".ToTemplateUri(),
                    refundRequest,
                    cancellationToken);
        }
    }
}
