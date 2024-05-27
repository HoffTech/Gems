// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

using System.Threading;
using System.Threading.Tasks;

using Gems.Mvc.GenericControllers;

using MediatR;

namespace Gems.Http.Samples.RequestMethods.Payments.GetPaymentStatus
{
    [Endpoint(
        "api/v1/payments/{id}/status",
        "GET",
        OperationGroup = "Payments",
        Summary = "Получение статуса платежа")]
    public class GetPaymentStatusQueryHandler(DefaultClientService defaultClientService)
        : IRequestHandler<GetPaymentStatusQuery, PaymentStatus>
    {
        public Task<PaymentStatus> Handle(GetPaymentStatusQuery query, CancellationToken cancellationToken)
        {
            return defaultClientService.GetAsync<PaymentStatus>(
                "api/payments/{id}/status".ToTemplateUri(query.PaymentId),
                cancellationToken);
        }
    }
}
