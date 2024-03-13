# Миграции Entity Framework

В настоящий момент поддерживается только миграция в Postgresql. 

```csharp
public static async Task<PostgreSqlContainer> MigrateAsync<TContext>(
    this PostgreSqlContainer contaner,
    CancellationToken cancellationToken = default)
    where TContext : DbContext
```

Применяет к контейнеру `contaner` миграцию `TContext`.

```csharp
public static async Task<ITestEnvironment> MigrateAsync<TContext>(
    this ITestEnvironment env,
    string name,
    CancellationToken cancellationToken = default)
    where TContext : DbContext
```

Применяет к контейнеру *Postgresql* с именем `name` миграцию `TContext`.

Пример использования:

```csharp
await using var env = await new TestEnvironmentBuilder()
    .UsePostgres(
        "MyProgresqlDatabase",
        (c, ct) => c.MigrateAsync<Context>(ct))
    .BuildAsync();
```
