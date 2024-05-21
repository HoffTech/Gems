// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using Gems.Http.Samples.Authentication.Basic.Payments.CreatePayment.BankApi.Dto;
using Gems.Http.Samples.Authentication.Basic.Payments.CreatePayment.BankApi.Options;
using Gems.Mvc.Filters.Errors;

using Microsoft.Extensions.Options;

namespace Gems.Http.Samples.Authentication.Basic.Payments.CreatePayment.BankApi
{
    public class BankService(IOptions<BankApiOptions> options, BaseClientServiceHelper helper)
        : BaseClientService<BusinessErrorViewModel>(options, helper)
    {
        public Task<CreatePaymentResponseDto> CreatePaymentAsync(
            CreatePaymentRequestDto requestDto,
            CancellationToken cancellationToken)
        {
            return this.PostAsync<CreatePaymentResponseDto>(
                "api/v1/bank/payments".ToTemplateUri(),
                requestDto,
                new Dictionary<string, string>
                {
                    ["Authorization"] = $"Basic {this.GetEncodedAuthorizationCredentials()}"
                },
                cancellationToken);
        }

        private string GetEncodedAuthorizationCredentials()
        {
            return Convert.ToBase64String(Encoding.UTF8.GetBytes($"{options.Value.UserName}:{options.Value.Password}"));
        }
    }
}
