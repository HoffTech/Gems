# Настройка Quartz в ITestApplicationBuilder

```csharp
public static ITestApplicationBuilder RemoveQuartzHostedService(this ITestApplicationBuilder builder)
```

Позволяет удалить сервис Quartz, чтобы заблокировать выполнение периодических заданий.