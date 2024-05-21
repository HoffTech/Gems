// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

using System.Threading;
using System.Threading.Tasks;

using Gems.Http.Samples.Authentication.Jwt.Payments.CreatePayment.BankApi.Dto;
using Gems.Http.Samples.Authentication.Jwt.Payments.CreatePayment.BankApi.Options;
using Gems.Mvc.Filters.Errors;

using Microsoft.Extensions.Options;

namespace Gems.Http.Samples.Authentication.Jwt.Payments.CreatePayment.BankApi
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
                cancellationToken);
        }

        protected override Task<string> GetAccessTokenAsync(CancellationToken cancellationToken)
        {
            return this
                .SendAuthenticationRequestAsync<string>(
                    templateUri: "api/v1/bank/login".ToTemplateUri(),
                    requestData: new LoginRequestDto
                    {
                        UserName = options.Value.UserName,
                        Password = options.Value.Password
                    },
                    headers: null,
                    cancellationToken);
        }
    }
}
