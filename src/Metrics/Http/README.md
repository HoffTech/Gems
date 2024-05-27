# Gems.Metrics.Http
Это библиотека .NET предоставляет возможность писать метрики для исходящих запросов.

Библиотека предназначена для следующих сред выполнения (и новее):

* .NET 6.0

# Содержание

* [Метрики с BaseClientService](#метрики-с-baseclientservice)


# Метрики с RequestMetricWriter
Библиотека предоставляет класс RequestMetricWriter, который используется внутри сервиса RequestMetricWriter.
Класс RequestMetricWriter пишет метрики для исходящих запросов. Пример:
```csharp
status_code_request{statusGroup="error",statusSubGroup="error_400",statusCode="409",requestUri="v1/some_uri" }
status_code_request{statusGroup="error",statusSubGroup="error_500",statusCode="501",requestUri="v1/some_uri" }
status_code_request{statusGroup="success",statusSubGroup="success_200",statusCode="201",requestUri="v1/some_uri" }

// время выполнения запроса
status_code_request_time{requestUri="v1/some_uri" }
```
Если нужно переопределить название метрики глобально для всех исходящих запросов, то зарегисрируйте тип метрики:
```csharp
services.AddHttpServices(
    this.Configuration, 
    options => options.StatusCodeMetricType = ApplicationMetricType.SalesReturnClaimStatusCodeRequest);
```
Тогда метрики будут писаться такие:
```csharp
sales_return_claim_status_code_request{statusGroup="error",statusSubGroup="error_400",statusCode="409",requestUri="v1/some_uri" }
sales_return_claim_status_code_request{statusGroup="error",statusSubGroup="error_500",statusCode="501",requestUri="v1/some_uri" }
sales_return_claim_status_code_request{statusGroup="success",statusSubGroup="success_200",statusCode="201",requestUri="v1/some_uri" }

// время выполнения запроса
sales_return_claim_status_code_request_time{requestUri="v1/some_uri" }
```
Если нужно переопределить название метрики для одного исходящего запроса, то запрос должен реализовывать интерфейс **IStatusCodeMetricAvailable**, а в поле **StatusCodeMetricType**, добавляемом интерфейсом, нужно передать тип метрики. Чтобы тип метрики не попал в запрос, ему нужно назначить аттрибут **JsonIgnoreAttribute**. Пример:
```csharp
public class MessageRequestDto : IStatusCodeMetricAvailable
{
    public string Id { get; set; }

    [JsonIgnore]
    public Enum StatusCodeMetricType { get; set; } = MetricsType.MyCoolMetric;
}
```
Метрики в таком случае будут писаться такие:
```csharp
my_cool_metric{statusGroup="error",statusSubGroup="error_400",statusCode="409",requestUri="v1/some_uri" }
my_cool_metric{statusGroup="error",statusSubGroup="error_500",statusCode="501",requestUri="v1/some_uri" }
my_cool_metric{statusGroup="success",statusSubGroup="success_200",statusCode="201",requestUri="v1/some_uri" }

// время выполнения запроса
my_cool_metric{requestUri="v1/some_uri" }
```
Если в uri присустсвуют динамические данные, например идентификаторы, то uri можно привести к шаблону.  
Например запрос пишет такие метрики:
```csharp
status_code_request{statusGroup="success",statusSubGroup="success_200",statusCode="201",requestUri="v1/orders/3fa85f64-5717-4562-b3fc-2c963f66afa6" }
status_code_request{statusGroup="success",statusSubGroup="success_200",statusCode="201",requestUri="v1/orders/52c3202d-2d34-40cd-b421-4295e4998a14" }
```
То необходимо uri привести к шаблону: **v1/orders/{orderId}**:
```csharp
var yourrUriAsTemplate = "v1/orders/{orderId}".ToTemplateUri("3fa85f64-5717-4562-b3fc-2c963f66afa6");
```
Метрика в таком случае будет писаться такая:
```csharp
status_code_request{statusGroup="success",statusSubGroup="success_200",statusCode="201",requestUri="v1/orders/{orderId}" }
```
[Пример кода](/src/Http/Http/samples/Gems.Http.Samples.UseTemplateUri)
