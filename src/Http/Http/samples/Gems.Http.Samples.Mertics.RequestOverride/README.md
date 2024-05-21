# Частное переопределение наименования метрики в BaseClientService для отдельного запроса

**Пример метрики по умолчанию**

- Статус код
```csharp
status_code_request{statusGroup="success",statusSubGroup="success_200",statusCode="200",requestUri="v1/samples/pong"} 1
```
- Время выполнения
```csharp
status_code_request_time{requestUri="v1/samples/pong"} 3112.0642
```

**Для того, чтобы переопределить метрику для запроса по умолчанию, необходимо**

1. Создать перечисление
```csharp
public enum PongMetricType
{
    PongRequestMetric
}
```

2. Унаследовать DTO от интерфейса `IStatusCodeMetricAvailable` и установить значение в поле `StatusCodeMetricType`
```csharp
public class PongDto : IStatusCodeMetricAvailable
{
    public string Secret { get; set; }

    [JsonIgnore]
    public Enum StatusCodeMetricType => PongMetricType.PongRequestMetric;
}
```

3. Вызвать запрос _BaseClientService_ и зафиксировать метрики с переопределенным наименованием на странице `http(s)://<your_domain>:<your_port>/metrics`  

Статус код
```csharp
pong_request_metric{statusGroup="success",statusSubGroup="success_200",statusCode="200",requestUri="v1/samples/pong"} 1
```
Время выполнения
```csharp
pong_request_metric_time{requestUri="v1/samples/pong"} 1469.4196
```

### Запуск примера
Вызовите ендпоинт с помощью **Swagger** `GET /api/v1/samples/ping`