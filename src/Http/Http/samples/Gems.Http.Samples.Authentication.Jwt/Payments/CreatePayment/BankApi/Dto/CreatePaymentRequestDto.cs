// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

using System;

namespace Gems.Http.Samples.Authentication.Jwt.Payments.CreatePayment.BankApi.Dto
{
    public record CreatePaymentRequestDto
    {
        public Guid Code { get; set; }

        public decimal Amount { get; set; }
    }
}
