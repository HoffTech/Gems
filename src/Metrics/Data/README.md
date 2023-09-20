# Gems.Metrics.Data
Это библиотека .NET содержит провайдер метрик запросов в бд.

Библиотека предназначена для следующих сред выполнения (и новее):

* .NET 6.0

# Содержание

* [Провайдер TimeMetricProvider](#провайдер-timemetricprovider)
* [Метрики с IUnitOfWork](#метрики-с-iunitofwork)

# Провайдер TimeMetricProvider
Провайдер предоставляет метод для создания метрики в соответветсвии названия функции или хранимой проедуры.
```csharp
TimeMetric GetTimeMetric(Enum timeMetricType, string functionName);
```
Данный провайдер используется в реализации интерфейса IUnitOwWork.

# Метрики с IUnitOfWork
Реализации IUnitOfWork используя класс TimeMetricProvider, автоматически пишут все временные метрики (TimeMetric) для хранимых процедур и функций. Пример:
```json
# HELP db_query_time Db Query Time
# TYPE db_query_time gauge
db_query_time{functionName="public_get_last_return_claim_log"} 2.4525
```
Можно переопределить глобальное имя метрики для указанного unit of work.    
Для этого необходимо настроить запись метрик для хранимых процедур и функций:
```csharp
services.AddPostgresqlUnitOfWork(
    "default", 
    options =>
    {
        //...
        options.DbQueryMetricType = DbQueryMetricType.SalesReturnClaimDbQueryTime; // ваше перечисление
        //...
    });

// Само перечисление:
public enum DbQueryMetricType
{
    SalesReturnClaimDbQueryTime
}

// ---------------------------------------------- ДОПОЛНИТЕЛЬНО: ------------------------------------------------------ 
// Вместо конфигурации опции DbQueryMetricInfo:
// options.DbQueryMetricInfo = DbQueryMetricType.SalesReturnClaimDbQueryTime;

// Можно указать:
// options.DbQueryMetricInfo = new MetricInfo { Name = "sales_return_claim_db_query_time", Description = "Sales Return Claim Db Query Time" };

// Или можно не указыаать данную опцию, тогда по умолчанию будет такая метрика
// options.DbQueryMetricInfo = new MetricInfo { Name = "db_query_time", Description = "Db Query Time" };
```
Для функции с названием public.get_last_return_claim_log будет писаться такая метрика sales_return_claim_db_query_time с меткой functionName
```json
# HELP sales_return_claim_db_query_time Sales Return Claim Db Query Time
# TYPE sales_return_claim_db_query_time gauge
sales_return_claim_db_query_time{functionName="public_get_last_return_claim_log"} 2.4525
```
Если необходимо изменить название метрики, описание метрики или название метки, то предваряем поле перечисления атрибутом MetricAttribute:
```csharp
public enum DbQueryMetricType
{
    [Metric(Name = "src_db_query", Description = "Time of sales_return_claim db query", LabelValues = new[] { "query_name" })]
    SalesReturnClaimDbQueryTime
}
```
Тогда метрика для функции public.get_last_return_claim_log запишется так:
```json
# HELP src_db_query Time of sales_return_claim db query
# TYPE src_db_query gauge
src_db_query{query_name="public_get_last_return_claim_log"} 2.4525
```

Так же можно передать свое значение перечисления непосредственно прям в метод выполнения sql запроса, что позволит переопределить поведение для кокретного запроса. Н-р:
```csharp
public enum DbQueryMetricType
{
    [Metric(Name = "get_last_return_claim_log", Description = "Time of getting last return_claim log"]
    GetLastReturnClaimLogTime
}

// вызов метода 
this.unitOfWorkProvider.GetUnitOfWork(cancellationToken).CallTableFunctionAsync<ReturnClaimLog>(
            "public_get_last_return_claim_log",
            new Dictionary<string, object>
            {
                // ...
            },
            DbQueryMetricType.GetLastReturnClaimLogTime);
```
В таком случае метрика для функции public.get_last_return_claim_log запишется так:
```json
# HELP get_last_return_claim_log Time of getting last return_claim log
# TYPE get_last_return_claim_log gauge
get_last_return_claim_log 2.4525
```
Если необходимо записать метрику для сырого запроса (Query), то так же можно использовать последний пример (передача перечисления напрямую в метод):
```csharp
public enum DbQueryMetricType
{
    [Metric(Name = "get_last_return_claim_log", Description = "Time of getting last return_claim log"]
    GetLastReturnClaimLogTime
}

// вызов метода 
this.unitOfWorkProvider.GetUnitOfWork(cancellationToken).QueryAsync<ReturnClaimLog>(
            "SELECT * FROM return_claim_log WHERE [some_condition]",
            DbQueryMetricType.GetLastReturnClaimLogTime);
```