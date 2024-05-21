// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

using System;
using System.Threading;
using System.Threading.Tasks;

using Gems.Http.Samples.SendRequestWithCustomError.Payments.CreateInvoice.PaymentApi;
using Gems.Http.Samples.SendRequestWithCustomError.Payments.CreateInvoice.PaymentApi.Dto;
using Gems.Mvc.GenericControllers;

using MediatR;

namespace Gems.Http.Samples.SendRequestWithCustomError.Payments.CreateInvoice
{
    [Endpoint(
        "api/v1/payments/create-invoice",
        "POST",
        OperationGroup = "Payments",
        Summary = "Запрос на создание Платежного счета")]
    public class CreateInvoiceCommandHandler(
        PaymentService paymentService)
        : IRequestHandler<CreateInvoiceCommand>
    {
        public async Task Handle(CreateInvoiceCommand command, CancellationToken cancellationToken)
        {
            await paymentService
                .CreateInvoiceAsync(
                    new InvoiceRequest
                    {
                        AccountId = await paymentService
                            .RegisterAccountAsync(user: Guid.NewGuid().ToString(), cancellationToken)
                            .ConfigureAwait(false)
                    },
                    cancellationToken)
                .ConfigureAwait(false);
        }
    }
}
