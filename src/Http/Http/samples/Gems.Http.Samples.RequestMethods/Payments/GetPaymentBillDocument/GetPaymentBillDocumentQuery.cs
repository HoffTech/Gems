// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

using MediatR;

using Microsoft.AspNetCore.Mvc;

namespace Gems.Http.Samples.RequestMethods.Payments.GetPaymentBillDocument
{
    public record GetPaymentBillDocumentQuery : IRequest<string>
    {
        [FromRoute(Name = "id")]
        public string BillId { get; set; }
    }
}
