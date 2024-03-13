# Настройка конфигурации в ITestApplicationBuilder

```csharp
public static ITestApplicationBuilder UseSetting<T>(this ITestApplicationBuilder builder, string name, T value)
```

Добавляет в настроки значение `value` с ключем `name`. `value` может быть как простым типом, так и объектом.

```csharp
public static ITestApplicationBuilder ReplaceSetting(
    this ITestApplicationBuilder builder,
    string path,
    string value)
```

Заменяет значение ключа `path` новым значением `value`.

```csharp
public static ITestApplicationBuilder ReplaceSetting(
    this ITestApplicationBuilder builder,
    string path,
    StringComparison stringComparison,
    string value)
```

Заменяет значение ключа `path` новым значением `value`. Алгоритм сравнения ключей задается через `stringComparison`.

```csharp
public static ITestApplicationBuilder ReplaceSetting(
    this ITestApplicationBuilder builder,
    string path,
    Func<string, string> valueFactory)
```

Заменяет значение ключа `path` используя фабрику `valueFactory`.

```csharp
public static ITestApplicationBuilder ReplaceSetting(
    this ITestApplicationBuilder builder,
    string path,
    StringComparison stringComparison,
    Func<string, string> valueFactory)
```

Заменяет значение ключа `path` используя фабрику `valueFactory`. Алгоритм сравнения ключей задается через `stringComparison`.

```csharp
public static ITestApplicationBuilder ReplaceSetting(
    this ITestApplicationBuilder builder,
    Func<string, bool> pathMatcher,
    Func<string, string> valueFactory)
```

Заменяет значение ключей используя функцию сравнения `pathMatcher` и фабрику `valueFactory`.

```csharp
public static ITestApplicationBuilder ReplaceSetting(
    this ITestApplicationBuilder builder,
    Regex pathRx,
    Func<string, string> valueFactory)
```

Заменяет значение ключей используя regex выражение `pathRx` и фабрику `valueFactory`.

```csharp
public static ITestApplicationBuilder ReplaceSetting(
    this ITestApplicationBuilder builder,
    Regex pathRx,
    string value)
```

Заменяет значение ключей используя regex выражение `pathRx` и значение `value`.

```csharp
public static ITestApplicationBuilder UseConnectionString(this ITestApplicationBuilder builder, string name, string value);

public static ITestApplicationBuilder UseConnectionString(this ITestApplicationBuilder builder, string name, Uri uri);
```

Выставляет строку соединения с ключем `name` в секции конфигурации `ConnectionStrings`.

## Примеры

Смена расписания триггеров Quartz так, чтобы они не выполнялись:

```csharp
testApplicationBuilder.ReplaceSetting(
    new Regex(@"^Jobs:Triggers:[^:]+$"),
    "0 0 1 * * 2099");
```
