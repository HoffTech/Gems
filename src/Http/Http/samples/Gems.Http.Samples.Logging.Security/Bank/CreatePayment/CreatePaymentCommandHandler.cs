// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

using System.Threading;
using System.Threading.Tasks;

using Gems.Http.Samples.Logging.Security.Bank.CreatePayment.Dto;
using Gems.Http.Samples.Logging.Security.Shared;
using Gems.Mvc.GenericControllers;

using MediatR;

namespace Gems.Http.Samples.Logging.Security.Bank.CreatePayment
{
    [Endpoint(
        "api/v1/bank/payments",
        "POST",
        OperationGroup = "Payments",
        Summary = "Запрос на создание платежа в Банке")]
    public class CreatePaymentCommandHandler : IRequestHandler<CreatePaymentCommand, PaymentDto>
    {
        public Task<PaymentDto> Handle(CreatePaymentCommand command, CancellationToken cancellationToken)
        {
            return Task.FromResult(
                new PaymentDto
                {
                    Code = command.Code,
                    Amount = command.Amount,
                    Status = PaymentStatus.Pending
                });
        }
    }
}
