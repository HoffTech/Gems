## Миграция на Gems 6.0.0.0

###### Делаем изменения в файле csproj

- Удаляем сторонние библиотеки (они подтянутся из Gems библиотек):
  - *MediatR*
  - *MediatR.Extensions.Microsoft.DependencyInjection*
  - *Dapper*
  - *Dapper.SqlBuilder*
  - *FluentValidation*
  - *FluentValidation.AspNetCore*
  - *FluentValidation.DependencyInjectionExtensions*
  - *Microsoft.Extensions.Caching.StackExchangeRedis*
  - *Newtonsoft.Json, Npgsql*
  - *prometheus-net*
  - *System.Data.SqlClient*

- Заменяем Gems библиотеки:
  - *Gems.ServiceStructure.Application* заменяем на две другие *Gems.Caching 6.0.0* и *Gems.SourceControl 6.0.0*.
  - *Gems.ServiceStructure.Domain* заменяем на две другие библиотеки - *Gems.BusinessRules 6.0.0* и *Gems.DomainEvents 6.0.0*.
  - *Gems.ServiceStructure.AuthenticationAndSwagger* заменяем на две другие библиотеки - *Gems.Authentication 6.0.0* и *Gems.Swagger 6.0.0*.
  - *Gems.Kafka.Hosting* заменяем на *Gems.MessageBrokers.Kafka 6.0.0*.
  - *Gems.Quartz* заменяем на Gems.Jobs.Quartz 6.0.0*.
  - *Gems.Hangfire* заменяем на *Gems.Jobs.Hangfire 6.0.0*.
  - *Gems.Data* заменяем на две другие библиотеки *Gems.Data.Npgsql 6.0.0* и *Gems.Data.SqlServer 6.0.0*.
  - *Gems.Metrics.Abstractions* удаляем.
  - *Gems.Jobs.Abstractions* удаляем.
  - Остальные *Gems* библиотеки обновляем до версии 6.0.0.
  - Добавляем библиотеку *Gems.HealthChecks 6.0.0*, если еще не была добавлена. 

- Обновляем сторонние библиотеки:
  - Библиотеку *AutoMapper* обновляем до версии 12.0.1.
  - Библиотеку *AutoMapper.Extensions.Microsoft.DependencyInjection* обновляем до версии 12.0.0.
  - Библиотеку *prometheus-net.AspNetCore* обновляем до версии 6.0.0.

###### Делаем изменения в файле Startup

- Удаляем все неиспользуемые неймспейсы, так как многие неймспейсы больше не нужны или переименовались.
- Импортируем все неймспейсы заново (наводим на некоторый красный метод, например на ***services.AddSwagger*** и импортируем все неймспейсы).
- Строчку ***services.AddMediatR(Assembly.GetExecutingAssembly());*** заменяем на:

```diff
-    services.AddMediatR(Assembly.GetExecutingAssembly());
+    services.AddMediatR(configuration => configuration.RegisterServicesFromAssemblyContaining<Startup>());
```

- Строчку ***services.AddHealthChecks();*** заменяем на:

```diff
-    services.AddHealthChecks();
+    services.AddDefaultHealthChecks();
```

- Строчку ***endpoints.MapHealthChecks("/health");*** заменяем на:

```diff
-    endpoints.MapHealthChecks("/health");
+    endpoints.MapDefaultHealthChecks();
```

- Строчку ***app.UseSwaggerWithAuthentication(this.Configuration);*** заменяем на строчки:

```diff
-    app.UseSwaggerWithAuthentication(this.Configuration);
+    app.UseAuthentication();
+    app.UseAuthorization();
+    app.UseSwaggerApi(this.Configuration);
```

- Добавим строчки в метод Configure, если еще не были добавлены:

```diff
+    services.AddHttpContextAccessor();
+    services.AddSecureLogging();
```

- Вызовы методов ***services.AddUnitOfWork*** заменяем на *services.AddPostgresqlUnitOfWork* *(если options.DbType не был указан или равен UnitOfWorkDbType.PostgreSql)* и *services.AddMsSqlUnitOfWork (равен UnitOfWorkDbType.MsSql)*. При этом удаляем использования *UnitOfWorkOptions.DbType*, так как сам метод уже говорит о типе UnitOfWOrk.

###### Делаем изменения в файле values.yaml
- Настраиваем хелсчек следующим образом:
```
Probes:
  liveness:
    path: /liveness
    port: http
  readiness:
    path: /readiness
    port: http
```
- Заменяем переменную swagger (если такая есть):
```diff
- AD__GitLabSwaggerPrefix: "__YOUR_VARIABLE_NAME__"
+ Swagger__GitLabSwaggerPrefix: "__YOUR_VARIABLE_NAME__"
```

###### Делаем изменения в файле appsettings.json
- Настройки AD переименовываем в Swagger. Оставляем только нужные настройки для Swagger. Пример:
```csharp
"Swagger": {
  "EnableAuthority": false,
  "SwaggerKey": "sales_return_claim_web_api",
  "SwaggerName": "Sales Return Claim Web API",
  "SwaggerSchema": "sales-return-claim",
  "GitLabSwaggerPrefix": "returnclaim",
  "EnableSchemaForErrorResponse": true
},
```

###### Рефакторим обработчики запросов и команд **MediatR**
- Меняем обработчики **MediatR**:
  - Заменяем наследование от ***IRequestHandler<YourCommand, Unit>*** на ***IRequestHandler<YourCommand>***
  - Заменяем в сигнатуре метода возвращаемый тип ***Task<Unit>*** на ***Task***
  - Заменяем строчку ***return Unit.Value;*** на ***return;***
  - Заменяем строчку ***return Task.FromResult(Unit.Value);*** на return ***Task.CompletedTask;***
  - В сигнатуре метода пайплайна ***Task<YourResponse> Handle(YourCommand command, RequestHandlerDelegate<YourResponse> next,   CancellationToken cancellationToken)*** меняем местами параметры ***next*** и ***cancellationToken***.

- Меняем команды и запросы **MediatR**:
  - Убираем наследование ***IRequestTimeMetric*** (метрики пишутся по умолчанию).
  - Вместо интерфейса ***IRequestTimeMetricExt*** используем *IRequestTimeMetric* если нужно переопределить название метрик.
  - Вместо интерфейса ***IRequestTransaction*** используем *IRequestUnitOfWork*.
  - Вместо интерфейса ***IRequestNotFoundExt*** используем *IRequestNotFound*.
  - Вместо интерфейса ***IRequestCacheExt*** используем *IRequestCache*.

###### Рефакторим классы, наследуемые от класса BaseClientService
- В реализации ***BaseClientService*** сервиса заменяем конструктор ***base***. Например:
```csharp
public YourService(
    IOptions<HttpClientServiceOptions> options,
    BaseClientServiceHelper baseClientServiceHelper)
: base(options, baseClientServiceHelper)
{
    this.options = options;
}
// Если в конструктор были внедрены IMetricsService, ILogger, IHttpClientFactory или IRequestLogsCollectorFactory,
// то их можно удалить, так как класс BaseClientServiceHelper уже содержит эти все экземпляры.
```

###### Подключаем интерфейсы и атрибуты из других неймспейсов
- Подключаем атрибуты из других неймспейсов:
  - Атрибут ***JobHandler*** подключаем из библиотеки *Gems.Jobs* (данная библиотеку не нужно отдельно подключать - подтягивается вместе с *Gems.Jobs.Quartz* или *Gems.Jobs.Hangfire*).
  - Атрибут ***PgType*** подключаем из библиотеки из *Gems.Data.Npgsql*.
  - Атрибут ***LogField*** подключаем из библиотеки *Gems.Logging.Mvc*.
  - Атрибут ***Metric*** подключаем из библиотеки *Gems.Metrics*.  

- Подключаем интерфейсы из других неймспейсов:
  - Интерфейс ***IRequestEndpointLogging*** подключаем из библиотеки *Gems.Logging.Mvc* (неймспейса *Gems.Logging.Mvc.Behaviors*).
  - Интерфейсы ***IBusinessRule*** подключаем из библиотеки *Gems.BusinessRules*.
  - Интерфейс ***IStatusCodeMetricAvailable*** подключаем из библиотеки *Gems.Metrics.Http* (данная библиотеку не нужно отдельно подключать - подтягивается вместе с *Gems.Http*).
  - Интерфейс ***IServicesConfiguration*** подключаем из библиотеки *Gems.Mvc*.
