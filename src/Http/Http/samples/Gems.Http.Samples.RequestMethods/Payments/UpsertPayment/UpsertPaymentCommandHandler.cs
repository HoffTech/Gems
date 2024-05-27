// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

using System.Threading;
using System.Threading.Tasks;

using Gems.Mvc.GenericControllers;

using MediatR;

namespace Gems.Http.Samples.RequestMethods.Payments.UpsertPayment
{
    [Endpoint(
        "api/v1/payments/upsert",
        "POST",
        OperationGroup = "Payments",
        Summary = "Создание или обновление платежа")]
    public class UpsertPaymentCommandHandler(DefaultClientService defaultClientService)
        : IRequestHandler<UpsertPaymentCommand, string>
    {
        public Task<string> Handle(UpsertPaymentCommand command, CancellationToken cancellationToken)
        {
            return defaultClientService.PutAsync<string>(
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
