// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

using System.Threading;
using System.Threading.Tasks;

using Gems.Mvc.GenericControllers;

using MediatR;

namespace Gems.Http.Samples.RequestMethods.Payments.GetPaymentBillDocument
{
    [Endpoint(
        "api/v1/payments/{id}/bill",
        "GET",
        OperationGroup = "Payments",
        Summary = "Получение платежного счета")]
    public class GetPaymentBillDocumentQueryHandler(DefaultClientService defaultClientService)
        : IRequestHandler<GetPaymentBillDocumentQuery, string>
    {
        public Task<string> Handle(GetPaymentBillDocumentQuery query, CancellationToken cancellationToken)
        {
            return defaultClientService.GetStringAsync(
                "api/payments/{id}/bill".ToTemplateUri(query.BillId),
                cancellationToken);
        }
    }
}
