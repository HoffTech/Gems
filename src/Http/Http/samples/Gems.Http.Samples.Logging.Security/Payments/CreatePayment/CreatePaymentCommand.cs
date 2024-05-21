// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

using Gems.Http.Samples.Logging.Security.Payments.CreatePayment.Dto;

using MediatR;

namespace Gems.Http.Samples.Logging.Security.Payments.CreatePayment
{
    public record CreatePaymentCommand : IRequest<PaymentDto>
    {
        public decimal Amount { get; set; }
    }
}
