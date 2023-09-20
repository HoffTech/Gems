# Gems.Data.Npgsql

Это библиотека .NET содержит расширения для работы с dapper, реализации unit of work для postgresql и автоматическую регистрацию мапперов.  
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

- Установите последнии версии nuget пакета Gems.Data.Npgsql через менеджер пакетов
- Добавьте следующие строки в конфигурацию сервисов
```csharp
services.AddPostgresqlUnitOfWork(
    "default", // ключ по умолчанию равен "default"
    options =>
    {
        options.ConnectionString = this.Configuration.GetConnectionString("<connection_string>"); // для ключа "default" значение берется из ConnectionStrings.DefaultConnection
        options.DbQueryMetricType = DbQueryMetricType.YourMetricDbQueryTime; // при указании метрики, будут писаться все временные метрики для хранимых процедур и функций.
        options.RegisterMappersFromAssemblyContaining<Startup>(); // регистрация мапперов, отмеченных атрибутом PgType.
    });
// добавляем второй unit of work
services.AddPostgresqlUnitOfWork(
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
"PostgresqlUnitOfWorks": [
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
# Pg Sql мапперы
В postgresql обычно поля именуются в undescore и для dto необходимо прописывать наименование полей в PgAttribute, н-р:
```csharp
public class Inventory
{
    [PgName("item_id")]
    public string ItemId { get; set; }

    [PgName("category_goods_code")]
    public string CategoryGoodsCode { get; set; }

    [PgName("category_item_code")]
    public string CategoryItemCode { get; set; }

    [PgName("item_characteristics")]
    public short ItemCharacteristics { get; set; }
}
```
Для того, чтобы данный тип зарегистрировался автоматически, достаточно промаркировать класс атрибутом - PgTypeAttribute:
```csharp
[PgType]
public class Inventory
{
    [PgName("item_id")]
    public string ItemId { get; set; }

    [PgName("category_goods_code")]
    public string CategoryGoodsCode { get; set; }

    // ...
}
```
Для того, чтобы dapper мог использовать тип, как входной параметр, необходимо прописывать: 
```csharp
NpgsqlConnection.GlobalTypeMapper.MapComposite(typeof(Inventory), "pg_inventory_type");
```
Но данную строку можно не писать, если в атрибуте PgTypeAttribute передать имя типа pg:
```csharp
[PgType("pg_inventory_type")]
public class Inventory
{
    [PgName("item_id")]
    public string ItemId { get; set; }

    [PgName("category_goods_code")]
    public string CategoryGoodsCode { get; set; }

    // ...
}
```

# Cкалярная функция
Расширение NpgsqlDapperHelper предоставляет методы для вызова скалярной функции postgresql:
```csharp
await using var connection = new NpgsqlConnection(this.options.Value.ConnectionString);
await connection.OpenAsync(cancellationToken).ConfigureAwait(false);
await using var transaction = await connection.BeginTransactionAsync(cancellationToken).ConfigureAwait(false);
var result = await NpgsqlDapperHelper.CallScalarFunctionAsync<T>(
    NpgsqlConnection connection,
    NpgsqlTransaction transaction,
    string functionName,
    DynamicParameters parameters,
    int commandTimeout,
    CancellationToken cancellationToken).ConfigureAwait(false);

// в библиотеке присутсвуют множество перегруженных версий для данного метода
```
# Табличная функция
Расширение NpgsqlDapperHelper предоставляет методы для вызова табличной функции postgresql:
```csharp
await using var connection = new NpgsqlConnection(this.options.Value.ConnectionString);
await connection.OpenAsync(cancellationToken).ConfigureAwait(false);
await using var transaction = await connection.BeginTransactionAsync(cancellationToken).ConfigureAwait(false);
var result = await NpgsqlDapperHelper.CallTableFunctionAsync<T>(
    NpgsqlConnection connection,
    NpgsqlTransaction transaction,
    string functionName,
    DynamicParameters parameters,
    int commandTimeout,
    CancellationToken cancellationToken).ConfigureAwait(false);

// в библиотеке присутсвуют множество перегруженных версий для данного метода
```
# Хранимая процедура
Расширение NpgsqlDapperHelper предоставляет методы для вызова хранимой процедуры postgresql:
```csharp
await using var connection = new NpgsqlConnection(this.options.Value.ConnectionString);
await connection.OpenAsync(cancellationToken).ConfigureAwait(false);
await using var transaction = await connection.BeginTransactionAsync(cancellationToken).ConfigureAwait(false);
await NpgsqlDapperHelper.CallStoredProcedureAsync(
    NpgsqlConnection connection,
    NpgsqlTransaction transaction,
    string storeProcedureName,
    DynamicParameters parameters,
    int commandTimeout,
    CancellationToken cancellationToken).ConfigureAwait(false);

// в библиотеке присутсвуют множество перегруженных версий для данного метода
```
# Sql выражение
Расширение NpgsqlDapperHelper предоставляет методы для выполнения sql выражения postgresql:
```csharp
await using var connection = new NpgsqlConnection(this.options.Value.ConnectionString);
await connection.OpenAsync(cancellationToken).ConfigureAwait(false);
await using var transaction = await connection.BeginTransactionAsync(cancellationToken).ConfigureAwait(false);
var result = await NpgsqlDapperHelper.QueryAsync<T>(
    NpgsqlConnection connection,
    NpgsqlTransaction transaction,
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