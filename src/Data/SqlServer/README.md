# Gems.Data.SqlServer

Это библиотека .NET содержит расширения для работы с dapper, реализации unit of work для mssql.  
Расширение позволяет удобно выполнять скалярные функции, табличный функции, хранимые процедуры и sql выражения.  
Реализация unit of work позволяет объединять все запросы в одной транзакции.  


Библиотека предназначена для следующих сред выполнения (и новее):

* .NET 6.0

# Содержание

* [Установка](#установка)
* [Pg Sql мапперы](#pg-sql-мапперы)
* [Cкалярная функция](#скалярная-функция)
* [Табличная функция](#табличная-функция)
* [Хранимая процедура](#хранимая-процедура)
* [Sql выражение](#sql-выражение)
* [Unit of work](#unit-of-work)
* [UnitOfWorkBehavior](#unitofworkbehavior)
* [Метрики](#метрики)

# Установка

- Установите последнии версии nuget пакета Gems.Data.SqlServer через менеджер пакетов
- Добавьте следующие строки в конфигурацию сервисов
```csharp
services.AddMsSqlUnitOfWork(
    "default", // ключ по умолчанию равен "default"
    options =>
    {
        options.ConnectionString = this.Configuration.GetConnectionString("<connection_string>"); // для ключа "default" значение берется из ConnectionStrings.DefaultConnection
        options.DbQueryMetricType = DbQueryMetricType.YourMetricDbQueryTime; // при указании метрики, будут писаться все временные метрики для хранимых процедур и функций.
    });
// добавляем второй unit of work
services.AddMsSqlUnitOfWork(
    "idl", // ключ по умолчанию равен "default"
    options =>
    {
        options.ConnectionString = this.Configuration.GetConnectionString("<connection_string>"); // для ключа "default" значение берется из ConnectionStrings.DefaultConnection
        options.DbQueryMetricType = DbQueryMetricType.YourMetricDbQueryTime; // при указании метрики, будут писаться все временные метрики для хранимых процедур и функций.
        options.SuspendTransaction = true; // Игнорировать создание транзакций (флаг needTransaction).     
    });
```
Так же все настройки, кроме вызова метода options.RegisterMappersFromAssemblyContaining<Startup>() можно перенести в appsettings.json
```csharp
"MsSqlUnitOfWorks": [
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
      "Key": "idl",
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
# Cкалярная функция
Расширение SqlDapperHelper предоставляет методы для вызова скалярной функции mssql:
```csharp
await using var connection = new NpgsqlConnection(this.options.Value.ConnectionString);
await connection.OpenAsync(cancellationToken).ConfigureAwait(false);
await using var transaction = await connection.BeginTransactionAsync(cancellationToken).ConfigureAwait(false);
var result = await SqlDapperHelper.CallScalarFunctionAsync<T>(
    SqlConnection connection,
    DbTransaction transaction,
    string functionName,
    DynamicParameters parameters,
    int commandTimeout,
    CancellationToken cancellationToken).ConfigureAwait(false);

// в библиотеке присутсвуют множество перегруженных версий для данного метода
```
# Табличная функция
Расширение SqlDapperHelper предоставляет методы для вызова табличной функции mssql:
```csharp
await using var connection = new NpgsqlConnection(this.options.Value.ConnectionString);
await connection.OpenAsync(cancellationToken).ConfigureAwait(false);
await using var transaction = await connection.BeginTransactionAsync(cancellationToken).ConfigureAwait(false);
var result = await SqlDapperHelper.CallTableFunctionAsync<T>(
    SqlConnection connection,
    DbTransaction transaction,
    string functionName,
    DynamicParameters parameters,
    int commandTimeout,
    CancellationToken cancellationToken).ConfigureAwait(false);

// в библиотеке присутсвуют множество перегруженных версий для данного метода
```
# Хранимая процедура
Расширение SqlDapperHelper предоставляет методы для вызова хранимой процедуры mssql:
```csharp
await using var connection = new NpgsqlConnection(this.options.Value.ConnectionString);
await connection.OpenAsync(cancellationToken).ConfigureAwait(false);
await using var transaction = await connection.BeginTransactionAsync(cancellationToken).ConfigureAwait(false);
await SqlDapperHelper.CallStoredProcedureAsync(
    SqlConnection connection,
    DbTransaction transaction,
    string storeProcedureName,
    DynamicParameters parameters,
    int commandTimeout,
    CancellationToken cancellationToken).ConfigureAwait(false);

// в библиотеке присутсвуют множество перегруженных версий для данного метода
```
# Sql выражение
Расширение SqlDapperHelper предоставляет методы для выполнения sql выражения mssql:
```csharp
await using var connection = new NpgsqlConnection(this.options.Value.ConnectionString);
await connection.OpenAsync(cancellationToken).ConfigureAwait(false);
await using var transaction = await connection.BeginTransactionAsync(cancellationToken).ConfigureAwait(false);
var result = await SqlDapperHelper.QueryAsync<T>(
    SqlConnection connection,
    DbTransaction transaction,
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