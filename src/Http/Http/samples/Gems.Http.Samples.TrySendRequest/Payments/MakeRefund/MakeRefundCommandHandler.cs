// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

using System;
using System.Threading;
using System.Threading.Tasks;

using Gems.Http.Samples.TrySendRequest.Payments.MakeRefund.PaymentApi;
using Gems.Http.Samples.TrySendRequest.Payments.MakeRefund.PaymentApi.Consts;
using Gems.Http.Samples.TrySendRequest.Payments.MakeRefund.PaymentApi.Dto;
using Gems.Mvc.GenericControllers;

using MediatR;

namespace Gems.Http.Samples.TrySendRequest.Payments.MakeRefund
{
    [Endpoint(
        "api/v1/payments/make-refund",
        "POST",
        OperationGroup = "Payments",
        Summary = "Запрос на возврат средств Пользователя")]
    public class MakeRefundCommandHandler(
        PaymentService paymentService)
        : IRequestHandler<MakeRefundCommand>
    {
        public async Task Handle(MakeRefundCommand command, CancellationToken cancellationToken)
        {
            var (_, error) = await paymentService
                .RefundAsync(
                    new RefundRequest
                    {
                        InvoiceNumber = Guid.NewGuid().ToString(),
                    },
                    cancellationToken)
                .ConfigureAwait(false);

            switch (error?.Error?.Code)
            {
                case RefundErrorCodes.NeedManualProcessing:
                    // Н-р Установить статус Требуется Ручная обработка
                    break;
                case RefundErrorCodes.RefundDeclined:
                    // Н-р Установить статус Отказ и отправить уведомление пользователю
                    break;
            }

            // Н-р Установить статус Успешно и отправить уведомление пользователю
        }
    }
}
