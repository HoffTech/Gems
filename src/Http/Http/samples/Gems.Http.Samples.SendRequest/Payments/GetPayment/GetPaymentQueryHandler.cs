// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

using System.Threading;
using System.Threading.Tasks;

using Gems.Http.Samples.SendRequest.Payments.GetPayment.Dto;
using Gems.Http.Samples.SendRequest.Payments.GetPayment.PaymentApi;
using Gems.Mvc.GenericControllers;

using MediatR;

namespace Gems.Http.Samples.SendRequest.Payments.GetPayment
{
    [Endpoint(
        "api/v1/payments/{code}",
        "GET",
        OperationGroup = "Payments",
        Summary = "Запрос на получение информации о Платеже")]
    public class GetPaymentQueryHandler(
        PaymentService paymentService)
        : IRequestHandler<GetPaymentQuery, PaymentDto>
    {
        public async Task<PaymentDto> Handle(GetPaymentQuery query, CancellationToken cancellationToken)
        {
            return new PaymentDto
            {
                Amount = 100,
                Code = query.Code,
                Status = await paymentService
                    .GetPaymentStatusAsync(query.Code, cancellationToken)
                    .ConfigureAwait(false),
            };
        }
    }
}
