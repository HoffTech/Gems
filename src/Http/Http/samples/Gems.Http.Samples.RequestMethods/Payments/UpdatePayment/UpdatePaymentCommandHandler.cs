// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

using System.Threading;
using System.Threading.Tasks;

using Gems.Mvc.GenericControllers;

using MediatR;

namespace Gems.Http.Samples.RequestMethods.Payments.UpdatePayment
{
    [Endpoint(
        "api/v1/payments/update",
        "POST",
        OperationGroup = "Payments",
        Summary = "Обновление платежа")]
    public class UpdatePaymentCommandHandler(DefaultClientService defaultClientService)
        : IRequestHandler<UpdatePaymentCommand>
    {
        public Task Handle(UpdatePaymentCommand command, CancellationToken cancellationToken)
        {
            return defaultClientService.PatchAsync(
                "api/payments/{id}".ToTemplateUri(command.PaymentId),
                command.Amount,
                cancellationToken);
        }
    }
}
