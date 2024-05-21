// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

using MediatR;

using Microsoft.AspNetCore.Mvc;

namespace Gems.Http.Samples.RequestMethods.Payments.GetPaymentDocument
{
    public record GetPaymentDocumentQuery : IRequest<FileStreamResult>
    {
        [FromRoute(Name = "id")]
        public string PaymentId { get; set; }
    }
}
