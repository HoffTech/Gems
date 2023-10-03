# Gems.Data.MySql

Это библиотека .NET содержит расширения для работы с dapper, реализации unit of work для mysql и автоматическую регистрацию мапперов.  
Расширение позволяет удобно выполнять скалярные функции, табличный функции, хранимые процедуры и sql выражения.  
Реализация unit of work позволяет объединять все запросы в одной транзакции.  


Библиотека предназначена для следующих сред выполнения (и новее):

* .NET 6.0

# Содержание

* [Установка](#установка)
* [MySql мапперы](#mysql-мапперы)
* [Cкалярная функция](#скалярная-функция)
* [Табличная функция](#табличная-функция)
* [Хранимая процедура](#хранимая-процедура)
* [Sql выражение](#sql-выражение)
* [Unit of work](#unit-of-work)
* [UnitOfWorkBehavior](#unitofworkbehavior)
* [Метрики](#метрики)

# Установка

- Установите последнии версии nuget пакета Gems.Data.MySql через менеджер пакетов
- Добавьте следующие строки в конфигурацию сервисов
```csharp
services.AddMySqlUnitOfWork(
    "default", // ключ по умолчанию равен "default"
    options =>
    {
        options.ConnectionString = this.Configuration.GetConnectionString("<connection_string>"); // для ключа "default" значение берется из ConnectionStrings.DefaultConnection
        options.DbQueryMetricType = DbQueryMetricType.YourMetricDbQueryTime; // при указании метрики, будут писаться все временные метрики для хранимых процедур и функций.
        options.RegisterMappersFromAssemblyContaining<Startup>(); // регистрация мапперов, отмеченных атрибутом PgType.
    });
// добавляем второй unit of work
services.AddMySqlUnitOfWork(
    "opd", // ключ по умолчанию равен "default"
    options =>
    {
        options.ConnectionString = this.Configuration.GetConnectionString("<connection_string>"); // для ключа "default" значение берется из ConnectionStrings.DefaultConnection
        options.DbQueryMetricType = DbQueryMetricType.YourMetricDbQueryTime; // при указании метрики, будут писаться все временные метрики для хранимых процедур и функций.
        options.SuspendTransaction = true; // Игнорировать создание транзакций (флаг needTransaction).     
    });
```
Так же все настройки, кроме вызова метода options.RegisterMappersFromAssemblyContaining<Startup>() можно перенести в appsettings.json
```csharp
"MySqlUnitOfWorks": [
    {
      "Key": "default",
      "Options": {
        "ConnectionString": "<connection_string>",
        "DbQueryMetricInfo": {
          "Name": "your_metric_db_query_time",
          "Description": "Your Metric Db uery Time"
        }
      }
    },
    {
      "Key": "opd",
      "Options": {
        "ConnectionString": "<connection_string>",
        "DbQueryMetricInfo": {
          "Name": "your_metric_db_query_time",
          "Description": "Your Metric Db uery Time"
        },
        "SuspendTransaction": true
      }
    }
  ]
```
В качестве "<connection_string>" можно указать ссылку на другой раздел в appsettings: "${ConnectionStrings.Dax}".
# MySql мапперы
В mysql обычно поля именуются в undescore и для dto необходимо прописывать наименование полей в ColumnAttribute, н-р:
```csharp
public class Inventory
{
    [Column("item_id")]
    public string ItemId { get; set; }

    [Column("category_goods_code")]
    public string CategoryGoodsCode { get; set; }

    [Column("category_item_code")]
    public string CategoryItemCode { get; set; }

    [Column("item_characteristics")]
    public short ItemCharacteristics { get; set; }
}
```
Для того, чтобы данный тип зарегистрировался автоматически, достаточно промаркировать класс атрибутом - MySqlTypeAttribute:
```csharp
[MySqlType]
public class Inventory
{
    [Column("item_id")]
    public string ItemId { get; set; }

    [Column("category_goods_code")]
    public string CategoryGoodsCode { get; set; }

    // ...
}
```
# Cкалярная функция
Расширение MySqlDapperHelper предоставляет методы для вызова скалярной функции mysql:
```csharp
await using var connection = new MySqlConnection(this.options.Value.ConnectionString);
await connection.OpenAsync(cancellationToken).ConfigureAwait(false);
await using var transaction = await connection.BeginTransactionAsync(cancellationToken).ConfigureAwait(false);
var result = await MySqlDapperHelper.CallScalarFunctionAsync<T>(
    MySqlConnection connection,
    MySqlTransaction transaction,
    string functionName,
    DynamicParameters parameters,
    int commandTimeout,
    CancellationToken cancellationToken).ConfigureAwait(false);

// в библиотеке присутсвуют множество перегруженных версий для данного метода
```
# Табличная функция
Расширение MySqlDapperHelper не поддерживает табличные функции и бросает исключение NotSupportedException.
Вместо табличных функций используйте хранимые процедуры.

# Хранимая процедура
Расширение MySqlDapperHelper предоставляет методы для вызова хранимой процедуры mysql:
```csharp
await using var connection = new MySqlConnection(this.options.Value.ConnectionString);
await connection.OpenAsync(cancellationToken).ConfigureAwait(false);
await using var transaction = await connection.BeginTransactionAsync(cancellationToken).ConfigureAwait(false);
await MySqlDapperHelper.CallStoredProcedureAsync(
    MySqlConnection connection,
    MySqlTransaction transaction,
    string storeProcedureName,
    DynamicParameters parameters,
    int commandTimeout,
    CancellationToken cancellationToken).ConfigureAwait(false);

// в библиотеке присутсвуют множество перегруженных версий для данного метода
```
# Sql выражение
Расширение MySqlDapperHelper предоставляет методы для выполнения sql выражения mysql:
```csharp
await using var connection = new MySqlConnection(this.options.Value.ConnectionString);
await connection.OpenAsync(cancellationToken).ConfigureAwait(false);
await using var transaction = await connection.BeginTransactionAsync(cancellationToken).ConfigureAwait(false);
var result = await MySqlDapperHelper.QueryAsync<T>(
    MySqlConnection connection,
    MySqlTransaction transaction,
    string storeProcedureName,
    DynamicParameters parameters,
    int commandTimeout,
    CancellationToken cancellationToken).ConfigureAwait(false);

// в библиотеке присутсвуют множество перегруженных версий для данного метода
```
# Unit of work
Как использовать смотрите в библиотеке [Gems.Data](/src/Data/Data/README.md)

# UnitOfWorkBehavior
Как использовать смотрите в библиотеке [Gems.Data](/src/Data/Data/README.md)

# Метрики
Как использовать смотрите в библиотеке [Gems.Data](/src/Data/Data/README.md)