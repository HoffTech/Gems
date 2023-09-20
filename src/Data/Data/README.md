# Gems.Data

Это библиотека .NET содержит интерфейс IUnitOfWork, интерфейс с реализацией IUnitOfWorkProvider, интерфейс с реализацией IConnectionStringProvider и пайплайн UnitOfWorkBehavior.  
Реализация IUnitOfWork позволяет удобно выполнять скалярные функции, табличный функции, хранимые процедуры и sql выражения.  
Реализация IUnitOfWork позволяет объединять все запросы в одной транзакции.  


Библиотека предназначена для следующих сред выполнения (и новее):

* .NET 6.0

# Содержание

* [Cкалярная функция](#скалярная-функция)
* [Табличная функция](#табличная-функция)
* [Хранимая процедура](#хранимая-процедура)
* [Sql выражение](#sql-выражение)
* [Unit of work](#unit-of-work)
* [UnitOfWorkBehavior](#unitofworkbehavior)
* [Метрики](#метрики)

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
# Unit of work
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
Команда должна быть унаследована от интерфейса IRequestTransaction. Если команда не будет унаследована от данного интерфейса, то транзакция не будет создаваться и в случае возникновения
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

# Метрики
В unit of work можно настроить автоматическую запись метрик для хранимых процедур и функций. Как работать с метриками с бд смотрите [здесь](/src/Metrics/Data/README.md#метрики-с-iunitofwork). 