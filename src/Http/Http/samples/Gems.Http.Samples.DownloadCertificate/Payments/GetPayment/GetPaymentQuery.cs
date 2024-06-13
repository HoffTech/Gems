// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

using System;

using Gems.Http.Samples.DownloadCertificate.Payments.GetPayment.Dto;

using MediatR;

using Microsoft.AspNetCore.Mvc;

namespace Gems.Http.Samples.DownloadCertificate.Payments.GetPayment
{
    public record GetPaymentQuery : IRequest<PaymentDto>
    {
        [FromRoute(Name = "code")]
        public Guid Code { get; set; }
    }
}
