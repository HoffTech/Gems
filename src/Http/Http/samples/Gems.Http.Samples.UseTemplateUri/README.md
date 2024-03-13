# Пример http сервиса на базе BaseClientService

Для того, чтобы отправить http запрос необходимо сделать следующее:

**Унаследоваться от класса BaseClientService<TDefaultError> с указанием типа ошибки по умолчанию TDefaultError**
```csharp
public class PongService : BaseClientService<string>
{
    private readonly IConfiguration configuration;

    public PongService(IConfiguration configuration, IOptions<HttpClientServiceOptions> options, BaseClientServiceHelper helper) : base(options, helper)
    {
        this.configuration = configuration;
    }
    // ...
}
```
**Переопределить свойство, отвечающее за базовый url**
```csharp
public class PongService : BaseClientService<string>
{
    // ...
    protected override string BaseUrl => this.configuration?.GetConnectionString("PongApiUrl") ?? throw new InvalidOperationException();
    // ...
}
```
**Создать метод, отвечающий за отправку http запроса**
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
Метод GetAsync определяет обобщенный тип выходных данных, как строка.  
Метод GetAsync принимает аргумент TemplateUri, отвечающий за эндпоинт, на который делается http запрос. Для этого у строки, определяющий uri, необходимо вызвать метод расширения ToTemplateUri и передать нужные аргументы для замены плейсхолдеров в uri.

