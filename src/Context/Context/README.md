# Gems.Context

Эта библиотека .NET предоставляет контекс для хранения данных, доступный внутри любых обработчиков MediatR.

Библиотека предназначена для следующих сред выполнения (и новее):

* .NET 6.0

### Создание контекста
```csharp
public class SomeClass
{
    private readonly IContextFactory contextFactory;

    public SomeClass(IContextFactory contextFactory)
    {
        this.contextFactory = contextFactory;
    }

    public void SomeMethod()
    {
        this.contextFactory.Create();
    }
}
```
### Удаление контекста
```csharp
public class SomeClass
{
    private readonly IContextFactory contextFactory;

    public SomeClass(IContextFactory contextFactory)
    {
        this.contextFactory = contextFactory;
    }

    public async Task SomeMethod()
    {
        await this.contextFactory.DisposeAsync();
    }
}
```
Вместе с удалением контекста диспозятся все объекты внутри контекста (в словаре Items), которые наследуют интерфейс IDisposable или IAsyncDisposable.

### Использование контекста
1. Добавить IContextAccessor в параметра конструктора:
```csharp
public class SomeClass
{
    private readonly IContextAccessor contextAccessor;

    public SomeClass(IContextAccessor contextAccessor)
    {
        this.contextAccessor = contextAccessor;
    }

    public void SomeMethod()
    {
        this.contextAccessor.Items["Key1"] = "data1";
    }

    public void OtherMethod()
    {
        var data = this.contextAccessor.Items["Key1"];
        Console.WriteLine($"data: {data}"); 
        // В консоли выведится: 
        // data: data1
    }
}
```

