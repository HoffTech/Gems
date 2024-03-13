# Использование ITestEnvironment

```csharp
public static T Component<T>(this ITestEnvironment env, string name)
```

Ищет компонент с типом `T` и именем `name`.

```csharp
public static IDatabaseContainer Database(this ITestEnvironment env, string name)
```

Ищет контейнер типа IDatabaseContainer и именем `name`.

```csharp
public static string DatabaseConnectionString(this ITestEnvironment env, string name)
```

Ищет контейнер типа IDatabaseContainer и именем `name` и возвращает строку соединения с базой данных для этого контейнера.