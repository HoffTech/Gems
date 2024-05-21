// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

using System;
using System.Text.Json.Serialization;

using Gems.Logging.Mvc.LogsCollector;

namespace Gems.Http.Samples.Logging.AddLogsFromPayload.Payments.CreatePayment.BankApi.Dto
{
    public record CreatePaymentRequestDto
    {
        [LogField]
        public Guid Code { get; set; }

        public decimal Amount { get; set; }

        [LogField]
        [JsonIgnore]
        public string UserName { get; set; }
    }
}
