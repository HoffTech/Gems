# Глобальное переопределение наименования метрик в BaseClientService

**Пример метрики по умолчанию**

- Статус код
```csharp
status_code_request{statusGroup="success",statusSubGroup="success_200",statusCode="200",requestUri="v1/samples/pong"} 1
```
- Время выполнения
```csharp
status_code_request_time{requestUri="v1/samples/pong"} 3112.0642
```

**Для того, чтобы переопределить глобальную метрику по умолчанию, необходимо**
1. Создать перечисление
```csharp
public enum ApplicationRequestsMetricType
{
   GlobalOverrideSampleStatusCodeRequest
}
```

2. Зарегистрировать перечисление из п.1 в опциях _BaseClientService_ в классе _Startup.cs_
```csharp
opt.AddHttpServices = () => services.AddHttpServices(
    configuration,
    // переопределение перечисления
    options => options.StatusCodeMetricType = ApplicationRequestsMetricType.GlobalOverrideSampleStatusCodeRequest);
```

3. Вызвать запрос BaseClientService и зафиксировать метрику с переопределенным наименованием на странице `http(s)://<your_domain>:<your_port>/metrics` 

Статус код
```csharp
global_override_sample_status_code_request_time{requestUri="v1/samples/{secret}/pong"} 1076.7841
```
Время выполнения
```csharp
global_override_sample_status_code_request{statusGroup="success",statusSubGroup="success_200",statusCode="200",requestUri="v1/Samples/UseTemplateUri/{secret}/pong"} 1
```

### Запуск примера
Вызовите ендпоинт с помощью **Swagger** `GET /api/v1/samples/ping`