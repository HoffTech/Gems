# Базовые методы ITestEnvironmentBuilder

```csharp
ITestEnvironment Build();
```

Создает синхронно окружение тестирования.

```csharp
Task<ITestEnvironment> BuildAsync(CancellationToken ct = default);
```

Создает aсинхронно окружение тестирования.

```csharp
ITestEnvironmentBuilder UseBootstraper(Func<ITestEnvironment, CancellationToken, Task> bootstrap);
```

Регистрирует асинхронный метод для настройки окружения сразу после его создания.

```csharp
ITestEnvironmentBuilder UseComponent<TComponent>(Func<TComponent> build) where TComponent : class;
```

Регистрирует функцию для создания компонента окружения тестирования. Если компонент поддерживает IDisposable или IAsyncDisposable, то он будет уничтожен вместе с уничтожением окружения.


```csharp
ITestEnvironmentBuilder UseComponent<TComponent>(Action<TComponent> setup) where TComponent : class;
```

Добавляет компонент в окружение тестирования и регистрирует метод для его настройки. Если компонент поддерживает IDisposable или IAsyncDisposable, то он будет уничтожен вместе с уничтожением окружения.
