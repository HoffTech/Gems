# Введение дополнительного логирования параметров тела запроса в BaseClientService

> Логирование дополнительных параметров позволяет удобно фильтровать данные в ELK по отдельным полям 

Для того, чтобы залогировать отдельные параметры тела запроса необходимо:
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
3. В _DTO_ запроса или ответа установите на необходимые для логирования свойства атрибут `[LogField]`
   - **RequestDto**
    ```csharp
    public record CreatePaymentRequestDto
    {
        // по умолчанию наименование поля в логах устанавливается от наименования свойства
        [LogField] 
        public Guid Code { get; set; }

        public decimal Amount { get; set; }

        // чтобы залогировать поле, которое нет необходимости передавать в запрос используется комбинация атрибутов [LogField] + [JsonIgnore]
        [LogField]
        [JsonIgnore]
        public string UserName { get; set; }
    }
    ```
    - **ResponseDto**
    ```csharp
    public record CreatePaymentResponseDto
    {
        public Guid Code { get; set; }

        public decimal Amount { get; set; }

        // чтобы переопределить наименование поля в логах, используется перегрузка конструктора
        [LogField("Payment-Status")] 
        public PaymentStatus Status { get; set; }
    }
    ```

4. Вызовите Http запрос и зафиксируйте логирование выбранных свойств с предустановленными атрибутами `[LogField]`
```json
{
  "@Timestamp": "2024-05-16 06:33:05.8027",
  "@level": "Info",
  "@host": "DESKTOP-L8MO3CK",
  "SourceContext": "Gems.Http.BaseClientServiceHelper",
  "@method": "WriteLogs",
  "MessageTemplate": "requestPath: {requestPath}, Code: {Code}, UserName:{UserName}, requestBody: {requestBody}, requestHeaders: {requestHeaders}, responseHeaders: {responseHeaders}, responseStatus: {responseStatus}, responseBody: {responseBody}, Payment-Status: {Payment-Status}, requestDuration: {requestDuration}",
  "requestPath": "http: //localhost:5135/api/v1/bank/payments",
  "Code": "207528a6-2a21-41d1-8a30-c7cc5b859bec", // Логирование поля из запроса
  "Payment-Status": 1, // Логирование поля из ответа
  "UserName": "d59d0acd-01a4-4258-b763-d19882ccb35b", // Логирование поля вне запроса и ответа
  "requestBody": "{\"code\":\"207528a6-2a21-41d1-8a30-c7cc5b859bec\",\"amount\":0}",
  "requestHeaders": {},
  "responseHeaders": {
    "Date": "Thu, 16 May 2024 06:33:05 GMT",
    "Server": "Kestrel",
    "Transfer-Encoding": "chunked"
  },
  "responseStatus": 200,
  "responseBody": "{\r\n  \"code\": \"207528a6-2a21-41d1-8a30-c7cc5b859bec\",\r\n  \"amount\": 0,\r\n\"status\": 1\r\n}",
  "requestDuration": 2898.6688
}
```

### Запуск примера
Вызовите ендпоинт с помощью **Swagger** `POST /api/v1/payments`
