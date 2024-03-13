# Настройка сервисов в ITestApplicationBuilder

```csharp
public static ITestApplicationBuilder RemoveServiceImplementation<TImplementation>(this ITestApplicationBuilder builder)
```

Убираетс сервис, если он реализован через `TImplementation`

```csharp
public static ITestApplicationBuilder RemoveService<TService>(this ITestApplicationBuilder builder)
```

Убирает сервис `TService`

```csharp
public static ITestApplicationBuilder RemoveServiceImplementationByFullName(this ITestApplicationBuilder builder, string implementationTypeFullName)
```
Убирает сервис, если он реализован через класс с полным квалифицированным именем `implementationTypeFullName`

```csharp
public static ITestApplicationBuilder RemoveServiceImplementationByName(this ITestApplicationBuilder builder, string implementationTypeName)
```
Убирает сервис, если он реализован через класс с именем `implementationTypeName`

```csharp
public static ITestApplicationBuilder RemoveServiceByFullName(this ITestApplicationBuilder builder, string serviceTypeFullName)
```

Убирает сервис с полным квалифицированным именем `serviceTypeFullName`

```csharp
public static ITestApplicationBuilder RemoveServiceByName(this ITestApplicationBuilder builder, string serviceTypeName)
```

Убирает сервис с именем `serviceTypeName`

```csharp
public static ITestApplicationBuilder ReplaceServiceWithSingleton<TService, TImplementation>(this ITestApplicationBuilder builder)
    where TService : class
    where TImplementation : class, TService
```

Заменяет сервис `TService` синглтоном, реализованным через `TImplementation`

```csharp
public static ITestApplicationBuilder ReplaceServiceWithSingleton<TService, TImplementation>(
    this ITestApplicationBuilder builder,
    Func<IServiceProvider, TImplementation> factory)
    where TService : class
    where TImplementation : class, TService
```

Заменяет сервис `TService` синглтоном, реализованным через `TImplementation` и созданным через фабрику `factory`.

```csharp
public static ITestApplicationBuilder ReplaceServiceWithScoped<TService, TImplementation>(this ITestApplicationBuilder builder)
    where TService : class
    where TImplementation : class, TService
```

Заменяет сервис `TService`, реализованным через `TImplementation` и временем жизни *scoped*

```csharp
public static ITestApplicationBuilder ReplaceServiceWithScoped<TService, TImplementation>(
    this ITestApplicationBuilder builder,
    Func<IServiceProvider, TImplementation> factory)
    where TService : class
    where TImplementation : class, TService
```

Заменяет сервис `TService`, реализованным через `TImplementation`, временем жизни *scoped* и созданным через фабрику `factory`.

```csharp
public static ITestApplicationBuilder ReplaceServiceWithTransient<TService, TImplementation>(this ITestApplicationBuilder builder)
    where TService : class
    where TImplementation : class, TService
```

Заменяет сервис `TService`, реализованным через `TImplementation` и временем жизни *transiend*

```csharp
public static ITestApplicationBuilder ReplaceServiceWithTransient<TService, TImplementation>(
    this ITestApplicationBuilder builder,
    Func<IServiceProvider, TImplementation> factory)
    where TService : class
    where TImplementation : class, TService
```

Заменяет сервис `TService`, реализованным через `TImplementation`, временем жизни *transiend* и созданным через фабрику `factory`.
