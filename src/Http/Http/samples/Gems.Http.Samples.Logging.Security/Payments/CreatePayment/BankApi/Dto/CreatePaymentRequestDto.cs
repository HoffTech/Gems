// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

using System;

using Gems.Logging.Security;

namespace Gems.Http.Samples.Logging.Security.Payments.CreatePayment.BankApi.Dto
{
    public record CreatePaymentRequestDto
    {
        public Guid Code { get; set; }

        public decimal Amount { get; set; }

        [Filter(@"(?<=.)[^@\n](?=[^@\n]*?[^@\n]@)|(?:(?<=@.)|(?!^)\G(?=[^@\n]*$)).(?=.*[^@\n]\.)", "*")]
        public string UserEmail { get; set; }

        [Filter]
        public string UserSecret { get; set; }
    }
}
