# Базовые методы ITestApplicationBuilder

```csharp
ITestApplication Build();
```

Создает тестовый экземпляр asp.net приложения.

```csharp
ITestApplicationBuilder ConfigureAppConfiguration(Action<WebHostBuilderContext, IConfigurationBuilder> setup);
```

Позволяет настроить конфигурацию приложения.


```csharp
ITestApplicationBuilder ConfigureLogging(Action<ILoggingBuilder> build);
```

Позволяет настроить логирование приложения.


```csharp
ITestApplicationBuilder ConfigureServices(Action<IServiceCollection> setup);
```

Позволяет настроить сервисы приложения. 


```csharp
ITestApplicationBuilder ConfigureWebHost(Action<IWebHostBuilder> setup);
```

Позволяет настроить среду выполнения приложения.


```csharp
ITestApplicationBuilder UseConfiguration(Func<IConfiguration> configurationFactory);
```

Позволяет задать конфигурацию приложения.


```csharp
ITestApplicationBuilder UseEnvironment(string environment);
```

Позволяет выбрать окружение исполнения приложения. Обычно это: Development, Staging, Production.

```csharp
ITestApplicationBuilder UseSetting(string name, string value);
```

Позволяет добавить ключ/значение в настройки приложения