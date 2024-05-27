// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

using System.Threading;
using System.Threading.Tasks;

using Gems.Http.Samples.Logging.Security.Payments.CreatePayment.BankApi.Dto;
using Gems.Http.Samples.Logging.Security.Payments.CreatePayment.BankApi.Options;
using Gems.Mvc.Filters.Errors;

using Microsoft.Extensions.Options;

namespace Gems.Http.Samples.Logging.Security.Payments.CreatePayment.BankApi
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
    }
}
