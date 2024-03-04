# Gems.TestInfrastructure

Библиотека Gems.TestInfrastructure предназначена для упрощения тестирования микросервисов путем создания тестового окружения.
Работа библиотеки основана на технологиях TestContainers, WireMock и WebApplicationFactory.

## Окружение

Для создания окружения теста используйте класс `TestEnvironmentBuilder`. Пример использования:

```csharp
await using var env = await new TestEnvironmentBuilder()
    .UsePostgres(
        "DefaultConnection",
        async (c, ct) =>
        {
            await c.ExecScriptAsync(new FileInfo(@"Resources/Sql/migration.sql"), ct);
        })
    .UseWireMockServer(
        "Default",
        server =>
        {
            server
                .Given(Request.Create()
                    .WithPath("/TemperatureInfo/*")
                    .UsingGet())
                .RespondWith(Response.Create()
                    .WithBody(@"{ ""temperature"": ""10.0"" }"));
        })
    .BuildAsync();
```

Окружение тестирования строится путем добавления контейнеров, которые могут быть, например, сервером базы данных, сервером WireMock или любым docker-контейнером. 
Смотрите раздел [Использование контейнеров](docs/EnvBuilderContainers.md)

Используйте [базовые методы конфигурации окружения](docs/EnvBuilderMethods.md) для добавления собственных компонентов в среду тестирования.

В результате создания окружения тестирования вы получаете экземпляр `ITestEnvironment`, который можете использовать для доступа к компонентам окружения. 
Смотрите раздел [Использование ITestEnvironment](docs/EnvMethods.md)


## Интеграционное тестирование

Для интеграционного тестирования сервиса asp.net используется класс `TestApplicationBuilder<TEntryPoint>`. Здесь `TEntryPoint` - это точка входа в ваше приложение. Обычно это класс `Program`.

Пример создания тестового приложения:

```csharp
using var app = new TestApplicationBuilder<Program>()
    .UseEnvironment(ConfigurationEnvironment.Development)
    .UseConnectionString("PosgresConnection", env)
    .LogToConsole()
    .LogSetMinimumLevel(LogLevel.Trace)
    .Build();
```

В дальнейшем вы можете отправлять Http запросы вашему приложению используя свойство `ITestApplication.HttpClient`.

Пример отправки запроса с использованием библиотеки [FluentHttpClient](https://github.com/scottoffen/fluenthttpclient):

```csharp
using var response = await app.HttpClient
    .UsingRoute("/")
    .GetAsync();
```

Для получения справки, по настройке и использованию интеграционного тестирования обратитесь к следующим разделам:

- [Базовые методы ITestApplicationBuilder](docs/AppBuilderBasicMethods.md)
- [Настройка конфигурации в ITestApplicationBuilder](docs/AppBuilderSettingsExtensions.md)
- [Настройка логирования в ITestApplicationBuilder](docs/AppBuilderLoggingExtensions.md)
- [Настройка сервисов в ITestApplicationBuilder](docs/AppBuilderServicesExtensions.md)
- [Настройка Quartz в ITestApplicationBuilder](docs/AppBuilderQuartzExtensions.md)

## Утилиты

Библиотека содержит различные утилиты, для упрощения тестирования приложения:

- [Утилиты для работы со схемой базы данных](docs/UtilsSchema.md)
- [Миграции Entity Framework](docs/EfMigrations.md)
- [Импорт из CSV](docs/CsvImport.md)
- [Расширения для Uri](docs/UriExtensions.md)

## Контроль

Для контроля тестов библиотека содержит расширения для [FluentAssertions](https://github.com/fluentassertions/fluentassertions).

Сравнение двух Json с использованием метода расширения `AsJson()`:

```csharp
@"{
    ""property1"": ""value1"",
    ""property2"": ""value2""
}"
.Should()
.AsJson()
.BeEquivalentTo(@"{
    ""property2"": ""value2"",
    ""property1"": ""value1""
}");
```

Сравнение схем данных с использованием методов расширения `Contain()`, `NotContain()`:

```csharp
var schema = await connection.SchemaAsync();
schema.CurrentDatabase
  .Tables
    .Should().Contain("products")
    .Which.Indexes
      .Should().NotContain("ix_product_category_id");
```


## Gitlab

Для запуска тестов в Gitlab следует добавить docker в качестве сервиса и 
настроить переменные окружения.
Пример:

```yaml
stages:
  - test

test:
  stage: test
  image: dotnet/sdk:6.0
  services:
    - docker:20.10.8-dind
  variables:
    DOCKER_DRIVER: overlay2
    DOCKER_TLS_CERTDIR: ""
    DOCKER_HOST: tcp://docker:2375
  script:
    - dotnet test
  tags:
    - dind
```

## Примеры использования

- [Микросервис WeatherInfo](../Samples/src/Gems.TestInfrastructure.Samples.WeatherInfo/)
- [Использование миграций EF](../Samples/src/Gems.TestInfrastructure.Samples.EfData/)