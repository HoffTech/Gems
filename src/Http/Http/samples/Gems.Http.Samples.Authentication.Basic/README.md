# Использование Basic аутентификации в запросах BaseClientService

**Для использования Basic аутентификации необходимо:**
1. Доавьте в конфигурацию _BaseClientService_ в _appsettings.json_ логин и пароль
```json
  "Payments": {
    "CreatePayment": {
      "BankApi": {
        // ...
        
        // Реквизиты Basic аутентификации
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
        /// Логин Basic аутентификации.
        /// </summary>
        public string UserName { get; init; }

        /// <summary>
        /// Пароль Basic аутентификации.
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

4. Вызовите **Http** запрос с помощью _BaseClientService_, при этом передав заголовок _Authorization_ в значении которого укажите строку формата `Basic {UserName}:{Password}` закодированную в _Base64_
```csharp
    public class BankService(IOptions<BankApiOptions> options, BaseClientServiceHelper helper)
        : BaseClientService<BusinessErrorViewModel>(options, helper)
    {
        public Task<CreatePaymentResponseDto> CreatePaymentAsync(
            CreatePaymentRequestDto requestDto,
            CancellationToken cancellationToken)
        {
            return this.PostAsync<CreatePaymentResponseDto>(
                "api/v1/bank/payments".ToTemplateUri(),
                requestDto,
                new Dictionary<string, string>
                {
                    // заголовок авторизации
                    ["Authorization"] = $"Basic {this.GetEncodedAuthorizationCredentials()}"
                },
                cancellationToken);
        }

        // метод получения значения для заголовка авторизации
        private string GetEncodedAuthorizationCredentials()
        {
            return Convert.ToBase64String(Encoding.UTF8.GetBytes($"{options.Value.UserName}:{options.Value.Password}"));
        }
    }
```