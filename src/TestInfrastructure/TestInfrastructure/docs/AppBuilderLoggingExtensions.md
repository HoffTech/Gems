# Настройка логирования в ITestApplicationBuilder

```csharp
public static ITestApplicationBuilder LogClearProviders(this ITestApplicationBuilder builder)
```

Очищает список провайдеров логирования.

```csharp
public static ITestApplicationBuilder LogSetMinimumLevel(this ITestApplicationBuilder builder, LogLevel level)
```

Выставляет минимальный уровень логирования.

```csharp
public static ITestApplicationBuilder LogToConsole(this ITestApplicationBuilder builder, Action<ConsoleLoggerOptions> setup = default)
```

Добавляет логирование в консоль. Используйте этот метод, если для тестов задействован NUnit.

```csharp
public static ITestApplicationBuilder LogToDebug(this ITestApplicationBuilder builder)
```

Добавляет отладочное логирование.