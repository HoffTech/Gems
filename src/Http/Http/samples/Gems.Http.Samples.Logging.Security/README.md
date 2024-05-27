# Сокрытие чувствительных данных при логировании Http запросов

> По умолчанию некоторые параметры скрываются и маскируются автоматически.  
> [Список чувствительных данных по умолчанию](http://api-dev.kifr-ru.local/securitykeys/static/log-filter.json)

Для того, чтобы сокрыть чувствительные данные тела запроса необходимо:
1. Подключите провайдер логов в _Program.cs_
```csharp
private static IHostBuilder CreateHostBuilder(string[] args)
{
    return Host.CreateDefaultBuilder(args)
        .UseServiceProviderFactory(new AutofacServiceProviderFactory())
        .ConfigureWebHostDefaults(webBuilder => webBuilder.UseStartup<Startup>())
        .ConfigureLogging(logging => logging.ClearProviders())
        .UseNLog(); // Регистрация провайдера NLog
}
```
2. Подключите сборщик логов в классе _Startup.cs_ (по умолчанию в Gems.CompositionRoot регистрируется `SecureRequestLogsCollector`)
```csharp
opt.AddSecureLogging = services.AddSecureLogging;
```
3. В _DTO_ запроса или ответа установите на необходимые для сокрытия свойства атрибут `[Filter]`
    ```csharp
    public record CreatePaymentRequestDto
    {
        public Guid Code { get; set; }

        public decimal Amount { get; set; }

        // Сокрытие эл. почты пользователя по маске (MyEmail@MyCompany.com -> M*****l@M*******y.com)
        [Filter(
            "(?<=.)[^@\\\\n](?=[^@\\\\n]*?[^@\\\\n]@)|(?:(?<=@.)|(?!^)\\\\G(?=[^@\\\\n]*$)).(?=.*[^@\\\\n]\\\\.)",
            "*")]
        public string UserEmail { get; set; }

        // Полное сокрытие значения
        // - если ReferenceType , то устанавливается значение Null
        // - если ValueType, то устанавливается значение default
        [Filter]
        public string UserSecret { get; set; }
    }
    ```

4. Вызовите Http запрос и зафиксируйте логирование выбранных свойств с предустановленными атрибутами `[Filter]`
```json
{
    "@Timestamp": "2024-05-17 06:00:50.0220",
    "@level": "Info",
    "@host": "DESKTOP-L8MO3CK",
    "SourceContext": "Gems.Http.BaseClientServiceHelper",
    "@method": "WriteLogs",
    "MessageTemplate": "requestPath: {requestPath}, requestBody: {requestBody}, requestHeaders: {requestHeaders}, responseHeaders: {responseHeaders}, responseStatus: {responseStatus}, responseBody: {responseBody}, requestDuration: {requestDuration}",
    "requestPath": "http: //localhost:5135/api/v1/bank/payments",
    // В requestBody поле userSecret было скрыто, а поле userEmail заменено символами по маске
    "requestBody": "{\"code\":\"aae4b87a-7606-459a-95b1-b6fcbe38f2c2\",\"amount\":120\"userEmail\":\"M*****l@MyCompany.com\"}",
    "requestHeaders": {},
    "responseHeaders": {
        "Date": "Fri, 17 May 2024 06:00:48 GMT",
        "Server": "Kestrel",
        "Transfer-Encoding": "chunked"
    },
    "responseStatus": 200,
    "responseBody": "{\"code\":\"aae4b87a-7606-459a-95b1-b6fcbe38f2c2\",\"amount\":120,\"status\":0}",
    "requestDuration": 6652.6032
}
```

### Запуск примера
Вызовите ендпоинт с помощью **Swagger** `POST /api/v1/payments`
