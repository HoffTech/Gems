// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

using System.Threading;
using System.Threading.Tasks;

using Gems.Mvc.GenericControllers;

using MediatR;

namespace Gems.Http.Samples.RequestMethods.Payments.CreatePayment
{
    [Endpoint(
        "api/v1/payments/create",
        "POST",
        OperationGroup = "Payments",
        Summary = "Создание платежа")]
    public class CreatePaymentCommandHandler(DefaultClientService defaultClientService)
        : IRequestHandler<CreatePaymentCommand, string>
    {
        public Task<string> Handle(CreatePaymentCommand command, CancellationToken cancellationToken)
        {
            return defaultClientService.PostAsync<string>(
                "api/payments".ToTemplateUri(),
                new
                {
                    command.ClientId,
                    command.Amount
                },
                cancellationToken);
        }
    }
}
