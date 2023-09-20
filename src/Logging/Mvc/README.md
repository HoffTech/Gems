# Gems.Logging.Mvc

Это библиотека .NET содержит реализации IPipelineBehavior и middleware для логгирования эндпоинтов.  

Библиотека предназначена для следующих сред выполнения (и новее):

* .NET 6.0

# Содержание

* [Установка](#установка)
* [Сбор логов RequestLogsCollector](#сбор-логов-requestlogscollector)
* [EndpointLoggingBehavior](#endpointloggingbehavior)
* [EndpointLoggingMiddleware](#endpointloggingmiddleware)
* [ScopeLoggingBehavior](#scopeloggingbehavior)

# Установка

- Установите последнюю версию nuget пакета Gems.Logging.Mvc через менеджер пакетов
- Другие настройки необходимы смотрите ниже для каждого пайплайна.
### Добавить шаблон в nlog
```xml
<target xsi:type="Console" name="logconsole">
	<layout xsi:type="JsonLayout" includeAllProperties="true">
		...
		<attribute name="MessageTemplate" layout="${message:raw=true}" escapeUnicode="false" />
		...
	</layout>
</target>
```

### Добавить HttpContextAccessor
```csharp
public void ConfigureServices(IServiceCollection services)
{
    services.AddHttpContextAccessor();
}
```
# Сбор логов RequestLogsCollector
Класс RequestLogsCollector позволяет записывать для запроса такую информацию:
- заголовки запроса(requestHeaders)
- тело запроса(requestBody)
- заголовки ответа(responseHeaders)
- тело ответа(responseBody)
- путь (requestPath)
- код статуса ответа (responseStatus).
- описание (endpointSummary)
- дополнительные данные из тела запроса или ответа (подробнее будет ниже)
Для этого имеются такие методы:
```csharp
// заголовки запроса
void AddRequestHeaders(Dictionary<string, string> headers);
// тело запроса
void AddRequest(object data);
// заголовки ответа
void AddResponseHeaders(Dictionary<string, string> headers);
// тело ответа
void AddResponse(object data);
// путь запроса (URL)
void AddPath(string path);
// код статуса ответа (http код)
void AddStatus(int status);
// описание эндпоинта
void AddEndpointSummary(string endpointSummary);
// дополнительные данные из тела запроса или ответа
void AddLogsFromPayload(object obj);

// после того, когда все данные добавлены необходимо вызвать следующий метод:
void WriteLogs();
```
Метод AddLogsFromPayload логгирует любые дополнительные данные из запроса или ответа. Для этого соответсвующему свойству модели запроса или ответа нужно добавить аттрибут **LogFieldAttribute**. Например:
```csharp
public class MyRequest // может быть и ответом
{
    [LogField]
    public string MyProperty { get; set; }
}
```
В примере выше именем параметра, который попадёт в лог, будет имя свойства. Можно изменить имя, передав его в конструктор аттрибута:
```csharp
 [LogField("my_field_name")]
```
Если нужно записать в лог данные, которые не будут отправлены в запросе, соответсвующему свойсту запроса нужно добавить ещё один аттрибут System.Text.Json.Serialization.**JsonIgnoreAttribute** (в большинстве случаев), либо **XmlIgnoreAttribute**, в зависимости от типа сообщения. Например:
```csharp
public class MyRequest // может быть и ответом
{
    [LogField]
    [JsonIgnore]
    public string MyProperty { get; set; }
}
```
# EndpointLoggingBehavior
Можно подключить автоматический сбор логов для обработчиков MediatR. Для включения этой возможности нужно выполнить конфигурацию следующим образом: 
### Добавить pipeline:
```csharp
public void ConfigureServices(IServiceCollection services)
{
    // должен быть в самом начале пайплайнов
    services.AddPipeline(typeof(EndpointLoggingBehavior<,>));
    // ...
}
```
### Добавить наследование от интерфейса **IRequestEndpointLogging**
Команда, запросы по которой нужно логгировать, должно имплементировать интерфейс **IRequestEndpointLogging**:
```csharp
public class MyCommand : IRequest, IRequestEndpointLogging
```

# EndpointLoggingMiddleware
Можно подключить автоматический сбор логов для логирования эндпоинтов, реализованных с помощью контроллеров (не Vertical Slice) вместо EndpointLoggingBehavior. Для включения этой возможности нужно выполнить конфигурацию следующим образом:
### Добавить middleware:
```csharp
public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {			
			app.UseEndpointLogging(); // должно быть перед UseEndpoints
			app.UseEndpoints(endpoints =>
            {
                // ...
            });
        }
```
Обратите внимение, что middleware для логгирования должно быть подключено перед UseEndpoints.

# ScopeLoggingBehavior
Имеется возможность добавлять к логам атрибут сферы действия цепочки логов - Scope. Атрибут представлен строкой по наименованию команды/запроса. Для включения этой возможности нужно выполнить конфигурацию следующим образом:

### Добавить behavior
```csharp
public void ConfigureServices(IServiceCollection services)
{
    // устанавливается в начало пайплайна, чтобы захватить логи в Scope
    services.AddPipeline(typeof(ScopeLoggingBehavior<,>));
    // ...
}
```
### Добавить шаблон в nlog (для Serilog корректировка конфигурации не требуется)
```xml
<target xsi:type="Console" name="logconsole">
	<layout xsi:type="JsonLayout" includeAllProperties="true">
        ...
        <attribute name="Scope" layout="${mdlc:Scope}" />
        ...
	</layout>
</target>
```
### Добавить наследование от интерфейса **IRequestScopeLogging**
Команда или запрос по которой нужно указание значения @Scope, должно имплементировать интерфейс **IRequestScopeLogging**:
```csharp
public class MyCommand : IRequest, IRequestScopeLogging
{
    public int Id { get; set; }

    public string GetScopeId()
    {
        // уникальный идентификатор для Scope,
        // это может быть как параметр запроса, так и константа
        return Id.ToString();
    }
}

```