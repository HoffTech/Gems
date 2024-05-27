// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

using System.Net.Mime;
using System.Threading;
using System.Threading.Tasks;

using Gems.Mvc.GenericControllers;

using MediatR;

using Microsoft.AspNetCore.Mvc;

namespace Gems.Http.Samples.RequestMethods.Payments.GetInvoiceDocument
{
    [Endpoint(
        "api/v1/payments/{id}/invoice",
        "GET",
        OperationGroup = "Payments",
        Summary = "Получение платежного поручения")]
    public class GetInvoiceDocumentQueryHandler(DefaultClientService defaultClientService)
        : IRequestHandler<GetInvoiceDocumentQuery, FileContentResult>
    {
        public async Task<FileContentResult> Handle(GetInvoiceDocumentQuery query, CancellationToken cancellationToken)
        {
            return new FileContentResult(
                await defaultClientService
                    .GetByteArrayAsync("api/v1/payments/{id}/invoice".ToTemplateUri(query.InvoiceId), cancellationToken)
                    .ConfigureAwait(false),
                MediaTypeNames.Application.Pdf);
        }
    }
}
