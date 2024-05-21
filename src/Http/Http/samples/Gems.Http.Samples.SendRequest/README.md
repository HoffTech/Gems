# Отправка запросов с помощью SendRequest и обработка ошибок

**SendRequestAsync** - метод отправки _Http_ запроса, который стоит применять если для обработки ошибок не предполагается дополнительной бизнес логики _(Н-р залогировать ошибку и пробросить исключение далее)_
```csharp
public Task<TaxStatusDto> GetTaxStatusAsync(
    string inn,
    CancellationToken cancellationToken)
{
    try
    {
        // Метод без префикса Try использует SendRequest
        return this
            .GetAsync<TaxStatusDto, string>(
                "api/v1/gos-uslugi/tax-status/{inn}".ToTemplateUri(inn),
                cancellationToken);
    }
    catch (RequestException<string> ex)
    {
        // Пример обработки исключений без использования Try запроса
        switch (ex.StatusCode)
        {
            case HttpStatusCode.NotFound:
                throw new RequestException(
                    "Не удалось получить налоговый статус. Данные о налоговом статусе отсутствуют.",
                    ex,
                    (HttpStatusCode)404);
            case HttpStatusCode.InternalServerError:
                throw new RequestException(
                    "Ошибка получения адреса. Сервис ГосУслуги не доступен",
                    ex,
                    (HttpStatusCode)499);
            default: throw;
        }
    }
```
