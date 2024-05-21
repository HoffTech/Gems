// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

using System;

using Gems.Http.Samples.Logging.AddLogsFromPayload.Shared;

namespace Gems.Http.Samples.Logging.AddLogsFromPayload.Payments.CreatePayment.Dto
{
    public record PaymentDto
    {
        public Guid Code { get; set; }

        public decimal Amount { get; set; }

        public PaymentStatus PaymentStatus { get; set; }
    }
}
