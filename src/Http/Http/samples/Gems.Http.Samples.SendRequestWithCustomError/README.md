# Переопределение возвращаемой ошибки с помощью SendRequestWIthCustomError

**SendWithCustomerErrorAsync** -  метод отправки _Http_ запроса, который позволяет переопределить тип возвращаемой ошибки для корректной десериализации и дальнейшей работой с ошибкой. 

> Метод применяется к отдельно взятому запросу, если формат его модели ошибки отличается от стандартного, определенного в параметрах типизации сервисного класса _BaseClientService&lt;BusinessErrorViewModel&gt;_, где _BusinessErrorViewModel_ ошибка по умолчанию

Пример реализации запроса с переопределением модели ошибки с помощью _SendWithCustomerErrorAsync_
```csharp
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
```