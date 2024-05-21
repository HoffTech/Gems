// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

using System;
using System.Threading;
using System.Threading.Tasks;

using AutoMapper;

using Gems.Http.Samples.Authentication.Jwt.Payments.CreatePayment.BankApi;
using Gems.Http.Samples.Authentication.Jwt.Payments.CreatePayment.BankApi.Dto;
using Gems.Http.Samples.Authentication.Jwt.Payments.CreatePayment.Dto;
using Gems.Mvc.GenericControllers;

using MediatR;

namespace Gems.Http.Samples.Authentication.Jwt.Payments.CreatePayment
{
    [Endpoint(
        "api/v1/payments",
        "POST",
        OperationGroup = "Payments",
        Summary = "Запрос на создание платежа")]
    public class CreatePaymentCommandHandler(BankService bankService, IMapper mapper)
        : IRequestHandler<CreatePaymentCommand, PaymentDto>
    {
        public async Task<PaymentDto> Handle(CreatePaymentCommand command, CancellationToken cancellationToken)
        {
            return mapper.Map<PaymentDto>(
                await bankService
                .CreatePaymentAsync(
                    new CreatePaymentRequestDto
                    {
                        Code = Guid.NewGuid(),
                        Amount = command.Amount
                    },
                    cancellationToken)
                .ConfigureAwait(false));
        }
    }
}
