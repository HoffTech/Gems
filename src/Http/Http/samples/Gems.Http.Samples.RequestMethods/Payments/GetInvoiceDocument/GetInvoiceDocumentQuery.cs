// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

using MediatR;

using Microsoft.AspNetCore.Mvc;

namespace Gems.Http.Samples.RequestMethods.Payments.GetInvoiceDocument
{
    public record GetInvoiceDocumentQuery : IRequest<FileContentResult>
    {
        [FromRoute(Name = "id")]
        public string InvoiceId { get; set; }
    }
}
