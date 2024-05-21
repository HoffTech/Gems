// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

using System;

using Gems.Http.Samples.Logging.Security.Bank.CreatePayment.Dto;

using MediatR;

namespace Gems.Http.Samples.Logging.Security.Bank.CreatePayment
{
    public record CreatePaymentCommand : IRequest<PaymentDto>
    {
        public Guid Code { get; set; }

        public decimal Amount { get; set; }
    }
}
