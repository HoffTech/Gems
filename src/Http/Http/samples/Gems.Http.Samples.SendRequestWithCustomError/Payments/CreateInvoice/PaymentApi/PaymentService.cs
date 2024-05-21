// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

using System.Net;
using System.Threading;
using System.Threading.Tasks;

using Gems.Http.Samples.SendRequestWithCustomError.Payments.CreateInvoice.PaymentApi.Dto;
using Gems.Http.Samples.SendRequestWithCustomError.Payments.CreateInvoice.PaymentApi.Options;
using Gems.Mvc.Filters.Errors;
using Gems.Mvc.Filters.Exceptions;

using Microsoft.Extensions.Options;

namespace Gems.Http.Samples.SendRequestWithCustomError.Payments.CreateInvoice.PaymentApi
{
    public class PaymentService(IOptions<PaymentApiOptions> options, BaseClientServiceHelper helper)
        : BaseClientService<BusinessErrorViewModel>(options, helper)
    {
        public Task<string> RegisterAccountAsync(string user, CancellationToken cancellationToken)
        {
            try
            {
                // запрос с ошибкой BusinessErrorViewModel по умолчанию
                return this
                    .PostAsync<string>(
                        "api/v1/payment/register-account".ToTemplateUri(),
                        user,
                        cancellationToken);
            }
            catch (RequestException<BusinessErrorViewModel> ex)
            {
                // обработка стандартной ошибки BusinessErrorViewModel
                switch (ex.StatusCode)
                {
                    case HttpStatusCode.InternalServerError:
                        throw new RequestException(
                            "Ошибка создания аккаунта. Сервис недоступен",
                            ex,
                            (HttpStatusCode)499);
                    default: throw;
                }
            }
        }

        public Task CreateInvoiceAsync(
            InvoiceRequest invoiceRequest,
            CancellationToken cancellationToken)
        {
            try
            {
                // запрос с переопределением ошибки на тип string
                return this
                    .PostWithCustomErrorAsync<Unit, string>(
                        "api/v1/payment/create-invoice".ToTemplateUri(),
                        invoiceRequest,
                        cancellationToken);
            }
            catch (RequestException<string> ex)
            {
                // обработка переопределенной ошибки string
                switch (ex.StatusCode)
                {
                    case HttpStatusCode.InternalServerError:
                        throw new RequestException(
                            "Ошибка создания счета. Сервис недоступен",
                            ex,
                            (HttpStatusCode)499);
                    default: throw;
                }
            }
        }
    }
}
