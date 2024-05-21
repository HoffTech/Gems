// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

using System;
using System.Threading;
using System.Threading.Tasks;

using AutoMapper;

using Gems.Http.Samples.Logging.AddLogsFromPayload.Payments.CreatePayment.BankApi;
using Gems.Http.Samples.Logging.AddLogsFromPayload.Payments.CreatePayment.BankApi.Dto;
using Gems.Mvc.GenericControllers;

using MediatR;

using PaymentDto = Gems.Http.Samples.Logging.AddLogsFromPayload.Payments.CreatePayment.Dto.PaymentDto;

namespace Gems.Http.Samples.Logging.AddLogsFromPayload.Payments.CreatePayment
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
                        Amount = command.Amount,
                        UserName = Guid.NewGuid().ToString()
                    },
                    cancellationToken)
                .ConfigureAwait(false));
        }
    }
}
