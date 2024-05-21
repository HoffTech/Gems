// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

using System.Threading;
using System.Threading.Tasks;

using Gems.Mvc.GenericControllers;

using MediatR;

namespace Gems.Http.Samples.RequestMethods.Payments.DeletePayment
{
    [Endpoint(
        "api/v1/payments/delete",
        "POST",
        OperationGroup = "Payments",
        Summary = "Удаление платежа")]
    public class DeletePaymentCommandHandler(DefaultClientService defaultClientService)
        : IRequestHandler<DeletePaymentCommand>
    {
        public Task Handle(DeletePaymentCommand command, CancellationToken cancellationToken)
        {
            return defaultClientService.DeleteAsync(
                "api/payments/{id}".ToTemplateUri(command.PaymentId),
                cancellationToken);
        }
    }
}
