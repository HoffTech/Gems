# Пример использования TemplateUri

Для того, чтобы отправить http запрос с использованием _TemplateUri_ необходимо cоздать метод, отвечающий за отправку http запроса
```csharp
public class PongService : BaseClientService<string>
{
    // ...
    public Task<string> GetPong(CancellationToken cancellationToken)
    {
        return this.GetAsync<string>("v1/Samples/UseTemplateUri/{secret}/pong".ToTemplateUri("ping"), cancellationToken);
    }
}
```
Метод _GetAsync_ принимает аргумент _TemplateUri_, отвечающий за эндпоинт, на который делается http запрос. Для этого у строки, определяющий _uri_, необходимо вызвать метод расширения _ToTemplateUri_ и передать нужные аргументы для замены плейсхолдеров в uri.

### Запуск примера
Вызовите ендпоинт с помощью **Swagger** `POST /v1/samples/ping`

