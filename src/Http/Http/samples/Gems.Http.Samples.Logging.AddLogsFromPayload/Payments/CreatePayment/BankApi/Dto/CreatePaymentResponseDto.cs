// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

using System;

using Gems.Http.Samples.Logging.AddLogsFromPayload.Shared;
using Gems.Logging.Mvc.LogsCollector;

namespace Gems.Http.Samples.Logging.AddLogsFromPayload.Payments.CreatePayment.BankApi.Dto
{
    public record CreatePaymentResponseDto
    {
        public Guid Code { get; set; }

        public decimal Amount { get; set; }

        [LogField("Payment-Status")]
        public PaymentStatus Status { get; set; }
    }
}
