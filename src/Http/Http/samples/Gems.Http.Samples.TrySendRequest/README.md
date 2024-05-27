# Отправка запросов с помощью TrySendRequest и обработка ошибок

**TrySendRequestAsync** -  метод отправки _Http_ запроса, который стоит применять если в случае возврата ошибки необходима обработка в виде дополнительной бизнес логики _(Н-р в случае ошибки устанавливать у сущности флаг ручной обработки, отправка умедомления и т.п.)_

1. Пример вызова метода с префиксом _Try_
   ```csharp
    public async Task<(Unit, BusinessErrorViewModel)> RefundAsync(
        RefundRequest refundRequest,
        CancellationToken cancellationToken)
    {
        return await this
            .TryPostAsync(
                "api/v1/payment/refund".ToTemplateUri(),
                refundRequest,
                cancellationToken);
    }
   ```
2. Обработка кода ошибки в поле `error?.Error?.Code` дополнительной бизнес логикой (см. комментарии)
   ```csharp
        public async Task Handle(MakeRefundCommand query, CancellationToken cancellationToken)
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
    ```
