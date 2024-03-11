# Gems.Data

Это библиотека .NET содержит интерфейс IUnitOfWork, интерфейс с реализацией IUnitOfWorkProvider, интерфейс с реализацией IConnectionStringProvider и пайплайн UnitOfWorkBehavior.  
Реализация IUnitOfWork позволяет удобно выполнять скалярные функции, табличный функции, хранимые процедуры и sql выражения.  
Реализация IUnitOfWork позволяет объединять все запросы в одной транзакции.  


Библиотека предназначена для следующих сред выполнения (и новее):

* .NET 6.0

# Содержание
* [Unit of work](#unit-of-work)
* [UnitOfWorkBehavior](#unitofworkbehavior)
* [Транзакция](#транзакция)
* [Cкалярная функция](#скалярная-функция)
* [Табличная функция](#табличная-функция)
* [Хранимая процедура](#хранимая-процедура)
* [Sql выражение](#sql-выражение)
* [Использование Linked Token](#использование-linked-token)
* [Использование локального контекста](#использование-локального-контекста)
* [Метрики](#метрики)
* [Работа с EF](#работа-с-ef)

# Unit of work
**[Пример кода](/src/Data/Data/samples/Gems.Data.Sample.UnitOfWork)**
```csharp
// объект unit of work получается из провайдера, в котором создается один раз и закрепляется за определенным cancellationToken.
// провайдер необходимо передать в конструктор, вызываемого класса: IUnitOfWorkProvider unitOfWorkProvider

var unitOfWork = this.unitOfWorkProvider.GetUnitOfWork("default", true, cancellationToken); 
// "defaut" - получить unit of work с ключом defaut (данный ключ является по умолчанию, можно не передавать данный аргумент)
// true - создать транзакцию. Второй раз можно получать без передачи данного параметра
// var unitOfWork = this.unitOfWorkProvider.GetUnitOfWork(cancellationToken);  

// вызов процедуры
await this.unitOfWorkProvider.GetUnitOfWork(cancellationToken).CallStoredProcedureAsync(
    "<store_procedure_name>",
    new Dictionary<string, object>
    {
        ["p_param1"] = "value1",
        ["p_param2"] = "value2",
    }).ConfigureAwait(false);

// вызов sql выражения
var items = await this.unitOfWorkProvider.GetUnitOfWork(cancellationToken).CallTableFunctionAsync<SomeItem>(
    "<function_name>",
    new Dictionary<string, object>
    {
        ["p_param1"] = "value1",
        ["p_param2"] = "value2",
    }).ConfigureAwait(false);
// каммит транзакции
await unitOfWork.CommitAsync();

// после выполнения транзакции, если объект unit of work больше не нужен, то его следует удалить:
// Dispose вызывается внутри данного метода.
await this.unitOfWorkProvider.RemoveUnitOfWork(cancellationToken);

// Объект unit of work можно использовать повторно. Внутри него просто повторно создаться подключение и транзакция.
```

# UnitOfWorkBehavior
Пайплайн UnitOfWorkBehavior создает unit of work для команды и закрепляет его за cancellationToken. После выполнения команды делается сommit транзакции и объект unit of work уничтожается.
Команда должна быть унаследована от интерфейса IRequestUnitOfWork. Если команда не будет унаследована от данного интерфейса, то транзакция не будет создаваться и в случае возникновения
исключения, операции в бд не будут откатываться.

Внутри команды все запросы к бд должны вызываться от объекта unit of work, который был закреплен за cancellationToken.
Объект unit of work можно получить через провайдер IUnitOfWorkProvider, который должен быть передан через конструктор.
Пример вызова хранимой процедуры
```csharp
await this.unitOfWorkProvider.GetUnitOfWork("default", cancellationToken).CallStoredProcedureAsync(
    "<store_procedure_name>",
    new Dictionary<string, object>
    {
        ["p_param1"] = "value1",
        ["p_param2"] = "value2",
    }).ConfigureAwait(false);
```
По умолчанию пайплайн UnitOfWorkBehavior может создать только один объект unit of work. Доменные обработчики (внутренние обработчики) не создают объект unit of work (даже если их команда или запрос наследуется от интерфейса IRequestUnitOfWork), а использует тот, который создался командой веб запроса или воркера. Смотрите разделы ниже "Использование Linked Token" и "Использование контекста" для того чтобы дать возможность доменному обработчику создавать свой объект unit of work.

# Транзакция
**[Пример кода](/src/Data/Data/samples/Gems.Data.Sample.Transaction)**

Возможность работы с транзакциями предоставляет интерфейс _IRequestUnitOfWork_

Пример регистрации
```csharp
    public class UpdatePersonCommand : IRequest, IRequestUnitOfWork
    {
        public string UpdatedBy { get; init; }

        public PersonDto Person { get; init; }
    }
```

Имплементировав данный интрфейс в команде/запросе операции в БД рассматриваются, как единое целое и в случае ошибки в ходе выполнения методов Обработчика операции в БД будут возвращены к исходному состоянию

# Cкалярная функция
```csharp
var result = await this.unitOfWorkProvider.GetUnitOfWork("default", cancellationToken)
    .CallScalarFunctionAsync<T>(
        string functionName,
        int commandTimeout,
        DynamicParameters parameters).ConfigureAwait(false);

// в библиотеке присутсвуют множество перегруженных версий для данного метода
```
# Табличная функция
```csharp
var result = await this.unitOfWorkProvider.GetUnitOfWork("default", cancellationToken)
    .CallTableFunctionAsync<T>(
        string functionName,
        int commandTimeout,
        DynamicParameters parameters).ConfigureAwait(false);

// в библиотеке присутсвуют множество перегруженных версий для данного метода
```
# Хранимая процедура
```csharp
await this.unitOfWorkProvider.GetUnitOfWork("default", cancellationToken)
    .CallStoredProcedureAsync(
        string storeProcedureName,
        int commandTimeout,
        DynamicParameters parameters).ConfigureAwait(false);

// в библиотеке присутсвуют множество перегруженных версий для данного метода
```
# Sql выражение
```csharp
var result = await this.unitOfWorkProvider.GetUnitOfWork("default", cancellationToken)
    .QueryAsync<T>(
        string storeProcedureName,
        int commandTimeout,
        DynamicParameters parameters).ConfigureAwait(false);

// в библиотеке присутсвуют множество перегруженных версий для данного метода
```

# Использование Linked Token
Связанные токены позволяют доменным обработчикам создать собственные объекты unit of work.
```csharp
using var linkedTokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
await this.mediator.Send(new SomeInnerCommand(), linkedTokenSource.Token);
```
Связанные токены является obsolete. Смотрите раздел "Использование контекста".

# Использование локального контекста
Провадер Unit of Work (UnitOfWorkProvider) можно сконфигурировать на работу с локальным контекстом. 
Тем самым делегировать источник хранения объектов Unit of Work на локальный контекст вместо собственного словаря (в UnitOfWorkProvider).
```json
"UnitOfWorkProviderOptions": {
    "UseContext": true
  }
```
Это добавляет возможность доменным обработчикам создать собственные объекты unit of work, что по факту избавляет нас от использования связанных токенов. Вместо такого кода:
```csharp
using var linkedTokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
await this.mediator.Send(new SomeInnerCommand(), linkedTokenSource.Token);
```
Можно теперь писать так:
```csharp
await this.mediator.Send(new SomeInnerCommand(), cancellationToken);
```

# Метрики
В unit of work можно настроить автоматическую запись метрик для хранимых процедур и функций. Как работать с метриками с бд смотрите [здесь](/src/Metrics/Data/README.md#метрики-с-iunitofwork).

# Работа с EF
Библиотека предоставляет DbContextProvider для доступа к дб контексту EF. DbContextProvider хранит дб контекст в локальном контексте.    
Это добавляет возможность доменным обработчикам создать собственные дб контексты.  
Если обработчику нужна транзакция, то необходимо предварять команду/запрос - интерфейсом IRequestUnitOfWork. Это избавляет разработчика от прямых вызовов SaveChangeAsync и CommitAsync. За все эти вызовы отвечает пайплайн UnitOfWorkBehavior.

Для того чтобы использовать DbContextProvider необходимо вместо регистрации дб контекста: 
```csharp
services.AddDbContext<SomeDbContext>(options => options.UseNpgsql(this.Configuration.GetConnectionString("connectionString")));
```
зарегистрировать фабрику дб контекста:
```csharp
services.AddDbContextFactory<SomeDbContext>(options => options.UseNpgsql(this.Configuration.GetConnectionString("connectionString")));
```
И зарегистрировать сам DbContextProvider:
```csharp
services.AddDbContextProvider();
```
Внимание: регистрировать провайдер не нужно, если уже был зарегистрирован любой unit of work.

Пример использования дб контекста:
```csharp
public class SomeCommand : IRequest, IRequestUnitOfWork
{
}

public class SomeCommandHandler : IRequestHandler<SomeCommand>
{
    private readonly IDbContextProvider dbContextProvider;

    public SomeCommandHandler(IDbContextProvider dbContextProvider)
    {
        this.dbContextProvider = dbContextProvider;
    }

    public async Task Handle(SomeCommand command, CancellationToken cancellationToken)
    {
        var dbContext = await this.dbContextProvider.GetDbContext<SomeDbContext>(cancellationToken).ConfigureAwait(false);
        // ...
    }
}
```


