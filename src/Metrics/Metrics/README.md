# Gems.Metrics

Это библиотека .NET является абстракцией над реализацией метрик. Предоставляет интерфейс  IMetricsService. 

Библиотека предназначена для следующих сред выполнения (и новее):

* .NET 6.0

# Содержание

* [Установка](#установка)
* [Счетчик](#счетчик)
* [Измеритель](#измеритель)
* [Гистограмма](#гистограмма)
* [Таймер](#таймер)
* [Сброс значения метрики](#сброс-значения-метрики)
* [Метки](#метки)
* [Установка метрики посредством структуры MetricInfo](#установка-метрики-посредством-структуры-metricinfo)
* [Установка метрики посредством перечисления MetricType](#установка-метрики-посредством-перечисления-metrictype)
* [Использование пайплайнов](#использование-пайплайнов)
* [ErrorMetricsBehavior](#errormetricsbehavior)
* [TimeMetricBehavior](#timemetricbehavior)
* [ResetMetricsBehavior](#resetmetricsbehavior)

# Установка

- Установите последнюю версию nuget пакета Gems.Metrics через менеджер пакетов
- Подключите реализацию интерфейса IMetricsService с помощью DI (смотрите в описании [библиотеки](/src/Metrics/Prometheus/README.md), реализующей данный интерфейс)

# Счетчик
Счетчик считает элементы за период времени. Cчетчик может только увеличивать или обнулять число. Счетчик не подходит для значений, которые могут уменьшаться, или для отрицательных значений.
```csharp
await this.metricsService.Counter("opd_update_orders", "opd update orders", 37).ConfigureAwait(false)
```
# Измеритель
Измеритель подходит для измерения текущего значения метрики, которое со временем может уменьшиться. 
```csharp
await this.metricsService.Gauge("order_num_in_current_queue", "Order number in current change tracking version queue", 8).ConfigureAwait(false);
```
# Гистограмма
Гистограмма — это более сложный тип метрики. Она предоставляет дополнительную информацию. Например, сумму измерений и их количество.
Значения собираются в области с настраиваемой верхней границей. 
```csharp
await this.metricsService.Histogram(
	(orderNewStatus.DateOfAdded - orderOldStatus.DateOfAdded).TotalSeconds, 
	"eq_business_orders_service_pickup_time_seconds", 
	"Время подбора").ConfigureAwait(false);
```	
Для корректной работы данной метрики может потребоваться произвести соответсвующие настройки (смотрите в описании библиотеки, реализующей интерфейс IMetricsService).
# Таймер
Таймер измеряет время какой-либо операции в коде.
```csharp
await using(var timeMetric = this.metricsService.Time("order_time_sec", "Time for loading each order")) 
{
	// ваш код
}
```
# Сброс значения метрики
В некоторых случаях метрики необходимо сбрасывать. Например, если у вас приложение работает в кластере и каждый экземпляр приложения конкурирует за взятие задачи на выполнение. Получается некоторые экземпляры могут простаивать и не брать долго задачи, и метрики будут показывать постоянно старые значения, которые уже не несут полезной информации. Чтобы сбросить метрику, необходимо ее просто установить в ноль.
```csharp
await this.metricsService.GaugeSet( "opd_opdb_errors", "opd opdb errors", 0).ConfigureAwait(false);
```
# Метки
Если у вас есть несколько метрик, которые вы хотите сложить/усреднить/суммировать, обычно это должна быть одна метрика с метками, а не несколько метрик.
Например, вместо http_responses_500_total и http_responses_403_total создайте одну метрику с именем http_responses_total с меткой кода для кода ответа HTTP. Затем вы можете обрабатывать всю метрику как одну в правилах и графиках.

Как правило, никакая часть имени метрики никогда не должна генерироваться процедурно. Вместо этого используйте метки.
Интерфейс  IMetricsService определяет каждый метод, принимающий в конце переменное число аргументов.

Например установить метрику с метками status и method (как настраивать названия меток смотрите в описании библиотеки, реализующей интерфейс IMetricsService). 
```csharp
await this.metricsService.GaugeSet( "http_responses_total", "http responses total", 1, "403", "POST").ConfigureAwait(false);
```

# Установка метрики посредством структуры MetricInfo
Интерфейс  IMetricsService содержит методы, принимающие структуру MetricInfo, которые позволяют более гибко установить метрику. В конечном итоге, каждый перегруженный метод вызывает внутри себя метод со структурой MetricInfo.
```csharp
public struct MetricInfo
{
    /// <summary>
    /// Описание метрики.
    /// </summary>
    public string Description { get; set; }

    /// <summary>
    /// Название метрики.
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// Название меток.
    /// </summary>
    public string[] LabelNames { get; set; }

    /// <summary>
    /// Значения для установки меток.
    /// </summary>
    public string[] LabelValues { get; set; }
}
```
Например для установки такой метрики:
```csharp
# HELP sales_return_claim_db_query_time Sales Return Claim Db Query Time
# TYPE sales_return_claim_db_query_time gauge
sales_return_claim_db_query_time{functionName="public_change_payment_return_external_transaction_id"} 2.8001
```
Необходимо написать такой код:
```csharp
await using (var timeMetric = this.metricsService.Time(new MetricInfo
{
    Description = "Sales Return Claim Db Query Time",
    Name = "sales_return_claim_db_query_time",
    LabelNames = new[] { "functionName" },
    LabelValues = new[] { "public_change_payment_return_external_transaction_id" }
}))
{
    // ваш код, который выполняется 2.8001 секунды
}
```

# Установка метрики посредством перечисления MetricType
Интерфейс  IMetricsService содержит методы, принимающие перечисления MetricType, которые позволяют установить метрику, с помощью одного параметра Enum, вместо передачи трех параметров (name, description и labelValues).
Использование перечисления предпочтительней, чем использование структуры MetricInfo или передачи параметров (name, description, labelValues), так как позволяет сгруппировать все метрики для фичи в одном месте и вызов метода, записывающий метрику выглядет более лакончино. 

Настройка метрики производится с помощью атрибута MetricAttribute:
```csharp
[AttributeUsage(AttributeTargets.Field)]
public class MetricAttribute : Attribute
{
    public string Description { get; set; }

    public string Name { get; set; }

    public string[] LabelValues { get; set; }

    public string[] LabelNames { get; set; }
}
```
Например для установки такой метрики:
```csharp
# HELP sales_return_claim_db_query_time Sales Return Claim Db Query Time
# TYPE sales_return_claim_db_query_time gauge
sales_return_claim_db_query_time{functionName="public_change_payment_return_external_transaction_id"} 2.8001
```
Необходимо написать такой код:
```csharp
// Cоздать перечисление
public enum SomeFeatureMetricType
{
    [Metric(Description = "Sales Return Claim Db Query Time", Name = "sales_return_claim_db_query_time", LabelNames = new[] { "functionName" }, LabelValues = new[] { "public_change_payment_return_external_transaction_id" })]
    ChangePaymentReturnExternalTransactionIdTime
}

// вызов метрики
await using (var timeMetric = this.metricsService.Time(SomeFeatureMetricType.ChangePaymentReturnExternalTransactionIdTime))
{
    // ваш код, который выполняется 2.8001 секунды
}
```
Наименование значения перечисления не имеет никакого смысла, если оно предваряется атрибутом MetricAttribute, которое определяет свойство Name. В примере выше значение ChangePaymentReturnExternalTransactionIdTime предваряется именем sales_return_claim_db_query_time. 
Если же Name не установлено, то название метрики будет сформировано на основании значения перечисления: ChangePaymentReturnExternalTransactionIdTime -> change_payment_return_external_transaction_id_time. 
Так же, если Description не установлено, то описание метрики будет сформировано на основании значения перечисления: ChangePaymentReturnExternalTransactionIdTime -> Change Payment Return External Transaction Id Time. 

LabelValues можно переопределить, если передать их непосредственно в метод. Например:
```csharp
// вызов метрики
await using (var timeMetric = this.metricsService.Time(SomeFeatureMetricType.ChangePaymentReturnExternalTransactionIdTime, "change_transaction_id"))
{
    // ваш код, который выполняется 2.8001 секунды
}
```
# Использование пайплайнов
Некоторые пайплайны применяются ко всем обработчикам, реализующих интерфейс IRequestHandler.
Это такие пайплайны:
- TimeMetricBehavior
- ErrorMetricsBehavior
- ResetMetricsBehavior

Регистрация пайплайнов:
```csharp
services.AddPipeline(typeof(ResetMetricsBehavior<,>));
services.AddPipeline(typeof(ErrorMetricsBehavior<,>));
services.AddPipeline(typeof(TimeMetricBehavior<,>));
```
# ErrorMetricsBehavior
Пайплайн ErrorMetricsBehavior отлавливает все исключения и регистрирует метрики. Пример для команды ImportInventTableCommand:
- errors_counter{feature_name="import_invent_table",error_type="business"} - Бизнесовые ошибки
- errors_counter{feature_name="import_invent_table",error_type="npgsql"} - Ошибки в бд 
- errors_counter{feature_name="import_invent_table",error_type="mssql"} - Ошибки в бд 
- errors_counter{feature_name="import_invent_table",error_type="validation"} - Ошибки валидации
- errors_counter{feature_name="import_invent_table",error_type="other"} - Другие ошибки

- feature_counters{feature_name="import_invent_table","status_code"="4xx"} - Ошибки 4xx (метка status_code >= 400 и <= 499)
- feature_counters{feature_name="import_invent_table","status_code"="5xx"} - Ошибки 5xx (метка status_code > 499)
- feature_counters{feature_name="import_invent_table",error_type="none","status_code"="200"} - Успешные

- import_invent_table_counter					- Все запуски
- import_invent_table_success_counter			- Успешные запуски

Где import_invent_table - это название команды/запроса без суффикса Command/Query 


# TimeMetricBehavior
Пайплайн TimeMetricBehavior регистрирует метрику времени выполнения запроса(команды), только если запрос(команда) унаследован от IRequestTimeMetric или IRequestTimeMetricExt. Название метрики устанавливается согласно названию запроса(команды).
Н-р: команда называется CheckPaymentCommand, то значит метрика будет называться check_payment_time.
Интерфейс IRequestTimeMetricExt позволяет переопределить название метрики:
```csharp
public interface IRequestTimeMetric
{
    Enum GetTimeMetricType();
}
```
Для того чтобы передать динамические метки метрик необходимо реализовать следующий интерфейс:
```csharp
public interface ILabelsProvider<in TRequest> : ILabelsProvider
    where TRequest : IBaseRequest
{
    string[] GetLabels(TRequest request);
}
```
Реализацию данного интерфейса необходимо зарегистрировать в сервисах.
```csharp
services.AddSingleton<ILabelsProvider<ImportInventTableCommand>, LabelsProvider>();
```
# ResetMetricsBehavior
Пайплайн ResetMetricsBehavior сбрасывает метрики через время указанное в appsettings:
```json
  "MetricsConfig": {
    "ResetMillisecondsDelay": 60000 // время, через которое необходимо сбрасывать метрики, по умолчанию 60 секунд
  }
```