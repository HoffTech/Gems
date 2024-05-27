// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

using System;

using MediatR;

namespace Gems.Http.Samples.RequestMethods.Payments.UpsertPayment
{
    public record UpsertPaymentCommand : IRequest<string>
    {
        public Guid ClientId { get; set; }

        public decimal Amount { get; set; }
    }
}
