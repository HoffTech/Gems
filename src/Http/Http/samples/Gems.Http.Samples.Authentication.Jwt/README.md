# Использование Jwt аутентификации в запросах BaseClientService

> Проверка на наличие токена работает автоматически перед каждым стандартным запросом  _(стандартный запрос - любой другой запрос, кроме аутентификации)_. Токен после первого получения сохраняется в приватное свойство **AccessToken**

> В случае получения ошибки **401** или **403** на стандартный запрос, значение в свойстве **AccessToken** устанавливается в **Null**, что впоследствии запускает повторный процесс получения токена при следующем  стандартном запросе

**Для использования Jwt аутентификации необходимо:**
1. Доавьте в конфигурацию _BaseClientService_ в _appsettings.json_ логин и пароль
```json
  "Payments": {
    "CreatePayment": {
      "BankApi": {
        // ...
        
        // Реквизиты Jwt аутентификации
        "UserName": "admin",                    // Логин
        "Password": "12345"                     // Пароль
      }
    }
  }
```

2. Добавьте в опциях _BaseClientService_ логин и пароль
```csharp
    public class BankApiOptions : HttpClientServiceOptions
    {
        /// <summary>
        /// Section in appsettings.json.
        /// </summary>
        public const string Name = "Payments:CreatePayment:BankApi";

        /// <summary>
        /// Логин Jwt аутентификации.
        /// </summary>
        public string UserName { get; init; }

        /// <summary>
        /// Пароль Jwt аутентификации.
        /// </summary>
        public string Password { get; init; }
    }
```

3. Зарегистрируйте опции и сервис
```csharp
public class CreatePaymentServicesConfiguration : IServicesConfiguration
{
    public void Configure(IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<BankApiOptions>(configuration.GetSection(BankApiOptions.Name));
        services.AddSingleton<BankService>();
    }
}
```

4. Переопределите метод _GetAccessTokenAsync_ в _BaseClientService_, используя запрос на получение токена с помощью метода _SendAuthenticationRequestAsync_.
```csharp
public class BankService(IOptions<BankApiOptions> options, BaseClientServiceHelper helper)
    : BaseClientService<BusinessErrorViewModel>(options, helper)
{
    // другие методы...
    
    // переопределние запроса
    protected override Task<string> GetAccessTokenAsync(CancellationToken cancellationToken)
    {
        return this
            .SendAuthenticationRequestAsync<string>(
                templateUri: "api/v1/bank/login".ToTemplateUri(),
                requestData: new LoginRequestDto
                {
                    // Логин и пароль из опций, зарегистрированных в п.2
                    UserName = options.Value.UserName,
                    Password = options.Value.Password
                },
                headers: null,
                cancellationToken);
    }
}
```

5. Вызовите Http запрос _BaseClientService_, в которой необходимо передать Jwt токен (он будет передан автоматически)
```csharp
public Task<CreatePaymentResponseDto> CreatePaymentAsync(
    CreatePaymentRequestDto requestDto,
    CancellationToken cancellationToken)
{
    return this.PostAsync<CreatePaymentResponseDto>(
        "api/v1/bank/payments".ToTemplateUri(),
        requestDto,
        cancellationToken);
}
```
