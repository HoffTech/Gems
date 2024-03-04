# Использование контейнеров в ITestEnvironmentBuilder

```csharp
public static ITestEnvironmentBuilder UseDockerContainer<TContainer>(
    this ITestEnvironmentBuilder builder,
    string name,
    Func<TContainer> factory,
    Func<TContainer, CancellationToken, Task> containerSetup = default)
    where TContainer : DockerContainer
```

Используйте данный метод для добавления docker-контейнера.

Здесь:

- `name` - наименование контейнера
- `factory` - метод для создания контейнера. Вы можете найти готовый контейнер на сайте библиотеки [TestContainers](https://dotnet.testcontainers.org/modules/)
- `containerSetup` - метод для конфигурации контейнера после его создания

## Postgres

```csharp
public static ITestEnvironmentBuilder UsePostgres(
    this ITestEnvironmentBuilder builder,
    string name,
    string image,
    Action<PostgreSqlBuilder> setupContainer = default,
    Func<PostgreSqlContainer, CancellationToken, Task> setupDatabase = default)
```

Добавляет в окружение тестирования контейнер Postresql.

Здесь:

- `name` - наименование контейнера
- `image` - образ docker, который использовать для контейнера. Если не указан, то используется *postgres:15-alpine*
- `setupContainer` - метод для настройки контейнера перед его созданием
- `setupDatabase` - метод для настройки базы данных после создания контейнера. В этом методе можно, например, выполнить миграции

После создания контейнера вы получаете экземпляр `PostgreSqlContainer`. Чтобы получить строку соединения с базой данных используйте свойство `PostgreSqlContainer.ConnectionString`.

```csharp
public static async Task<NpgsqlConnection> ConnectPostgresAsync(
    this ITestEnvironment env,
    string name,
    CancellationToken cancellationToken = default)
```

Позволяет создать соединение с базой данных по имени

```csharp
public static async Task<NpgsqlConnection> ConnectPostgresAsync(
    this IDatabaseContainer container,
    CancellationToken cancellationToken = default)
```

Позволяет создать соединение используя компонент `IDatabaseContainer`.


```csharp
public static async Task ExecScriptAsync(this PostgreSqlContainer c, FileInfo fileInfo, CancellationToken ct = default)
```

Выполняет sql-скрипт из файла `fileInfo`.

```csharp
public static async Task SetupAsync(
    this PostgreSqlContainer c,
    Func<NpgsqlConnection, PostgresSchema, Task> setup,
    CancellationToken ct = default)
```

Создает соединение к базе данных, получает ее текущую схему и передает в функцию настройки `setup`.

```csharp
public static async Task DoAsync(
    this PostgreSqlContainer c,
    Func<NpgsqlConnection, Task> action,
    CancellationToken ct = default)
```

Создает соединение к базе данных и передает его в обработчик `action`.

## MsSql

```csharp
public static ITestEnvironmentBuilder UseMsSql(
    this ITestEnvironmentBuilder builder,
    string name,
    string image,
    Action<MsSqlBuilder> setupContainer = default,
    Func<MsSqlContainer, CancellationToken, Task> setupDatabase = default)
```

Добавляет в окружение тестирования контейнер MsSql.

Здесь:

- `name` - наименование контейнера
- `image` - образ docker, который использовать для контейнера
- `setupContainer` - метод для настройки контейнера перед его созданием
- `setupDatabase` - метод для настройки базы данных после создания контейнера. В этом методе можно, например, выполнить миграции

После создания контейнера вы получаете экземпляр `MsSqlContainer`. Чтобы получить строку соединения с базой данных используйте свойство `MsSqlContainer.ConnectionString`.


```csharp
public static async Task<SqlConnection> ConnectMsSqlAsync(
    this ITestEnvironment env,
    string name,
    CancellationToken cancellationToken = default)
```

Создает соединение с базой данных используя имя компонеты.

```csharp
public static async Task<SqlConnection> ConnectMsSqlAsync(
    this IDatabaseContainer container,
    CancellationToken cancellationToken = default)
```

Создает соединение с базой данных используя экземпляр  `IDatabaseContainer`.

```csharp
public static async Task ExecScriptAsync(this MsSqlContainer c, FileInfo fileInfo, CancellationToken ct = default)
```

Выполняет скрипт из файла `fileInfo`.

```csharp
public static async Task SetupAsync(
    this MsSqlContainer c,
    Func<SqlConnection, MsSqlSchema, Task> setup,
    CancellationToken ct = default)
```

Создает соединение к базе данных, получает ее текущую схему и передает в функцию настройки `setup`.


```csharp
public static async Task DoAsync(
    this MsSqlContainer c,
    Func<SqlConnection, Task> action,
    CancellationToken ct = default)
```

Создает соединение к базе данных и передает его в обработчик `action`.

## Redis

```csharp
public static ITestEnvironmentBuilder UseRedis(
    this ITestEnvironmentBuilder builder,
    string name,
    string image,
    Action<RedisBuilder> setupContainer = default,
    Func<RedisContainer, CancellationToken, Task> setupDatabase = default)
```

Добавляет в окружение тестирования контейнер Redis.

Здесь:

- `name` - наименование контейнера
- `image` - образ docker, который использовать для контейнера
- `setupContainer` - метод для настройки контейнера перед его созданием
- `setupDatabase` - метод для настройки базы данных после создания контейнера. В этом методе можно, например, добавить данные

После создания контейнера вы получаете экземпляр `RedisContainer`. Чтобы получить строку соединения с базой данных используйте свойство `RedisContainer.ConnectionString`.

## WireMock

```csharp
public static ITestEnvironmentBuilder UseWireMockServer(
    this ITestEnvironmentBuilder builder,
    string name,
    WireMockServerSettings settings = default,
    Action<WireMockServer> setupServer = default)
```

Добавляет в окружение тестирования сервер WireMock.

Здесь:

- `name` - наименование контейнера
- `settings` - настройки сервера
- `setupServer` - метод для сервера. Здесь вы можете добавить настроить запросы/ответы сервера.

После создания контейнера вы получаете экземпляр `WireMockServer`. Для настройки вашего приложения используйте методы:

```csharp
public static WireMockServer WireMockServer(this ITestEnvironment env, string name)
```

Возвращает по имени экземпляр сервера WireMock.

```csharp
public static string WireMockServerUrl(this ITestEnvironment env, string name)
```

Возвращает по имени url сервера WireMock.

```csharp
public static Uri WireMockServerUri(this ITestEnvironment env, string name)
```

Возвращает по имени объект `Uri` сервера WireMock.