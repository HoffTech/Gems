﻿// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

using System;

using Gems.Http.Samples.Logging.Security.Shared;

namespace Gems.Http.Samples.Logging.Security.Payments.CreatePayment.BankApi.Dto
{
    public record CreatePaymentResponseDto
    {
        public Guid Code { get; set; }

        public decimal Amount { get; set; }

        public PaymentStatus Status { get; set; }
    }
}
