// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

using System.Net.Mime;
using System.Threading;
using System.Threading.Tasks;

using Gems.Mvc.GenericControllers;

using MediatR;

using Microsoft.AspNetCore.Mvc;

namespace Gems.Http.Samples.RequestMethods.Payments.GetPaymentDocument
{
    [Endpoint(
        "api/v1/payments/{id}/document",
        "GET",
        OperationGroup = "Payments",
        Summary = "Получение платежного документа")]
    public class GetPaymentDocumentQueryHandler(DefaultClientService defaultClientService)
        : IRequestHandler<GetPaymentDocumentQuery, FileStreamResult>
    {
        public async Task<FileStreamResult> Handle(GetPaymentDocumentQuery query, CancellationToken cancellationToken)
        {
            return new FileStreamResult(
                await defaultClientService
                    .GetStreamAsync(
                        "api/payments/{id}/document".ToTemplateUri(query.PaymentId),
                        cancellationToken)
                    .ConfigureAwait(false),
                MediaTypeNames.Application.Pdf);
        }
    }
}
