// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

using Gems.Http.Samples.SendRequest.Payments.GetPayment.PaymentApi.Options;
using Gems.Mvc.Filters.Errors;
using Gems.Mvc.Filters.Exceptions;

using Microsoft.Extensions.Options;

namespace Gems.Http.Samples.SendRequest.Payments.GetPayment.PaymentApi
{
    public class PaymentService(IOptions<PaymentApiOptions> options, BaseClientServiceHelper helper)
        : BaseClientService<BusinessErrorViewModel>(options, helper)
    {
        public Task<PaymentStatus> GetPaymentStatusAsync(
            Guid id,
            CancellationToken cancellationToken)
        {
            try
            {
                return this
                    .GetAsync<PaymentStatus>(
                        "api/payments/{id}/status".ToTemplateUri(id.ToString()),
                        cancellationToken);
            }
            catch (RequestException<BusinessErrorViewModel> ex)
            {
                switch (ex.StatusCode)
                {
                    case HttpStatusCode.NotFound:
                        throw new RequestException(
                            $"Не удалось получить статус платежа. Данные по коду \"{id}\" отсутствуют.",
                            ex,
                            (HttpStatusCode)404);
                    case HttpStatusCode.InternalServerError:
                        throw new RequestException(
                            "Ошибка получения статуса платежа. Сервис PaymentApi не доступен.",
                            ex,
                            (HttpStatusCode)499);
                    default: throw;
                }
            }
        }
    }
}
